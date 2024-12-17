using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace Application.AIML
{
    public class DiseasePredictionBasedOnSymptoms
    {
        private readonly MLContext mlContext;
        private ITransformer model;
        private List<string> symptomNames;
        private List<SymptomData> diseaseData;

        public DiseasePredictionBasedOnSymptoms()
        {
            mlContext = new MLContext();
            symptomNames = new List<string>();
        }

        public void TrainModel()
        {
            string filePath = Path.GetFullPath("symbipredict_2022.csv");
            var dataView = mlContext.Data.LoadFromTextFile<SymptomData>(filePath, separatorChar: ',', hasHeader: true);
            ExtractSymptomNames(filePath);
            LoadDiseaseData(filePath); 

            var pipeline = mlContext.Transforms.Concatenate("Features", nameof(SymptomData.Symptoms))
                .Append(mlContext.Transforms.Conversion.MapValueToKey("Label", nameof(SymptomData.Disease)))
                .Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy("Label", "Features"))
                .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            model = pipeline.Fit(dataView);
            Console.WriteLine("Model trained successfully.");
        }

        private void ExtractSymptomNames(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            {
                var header = reader.ReadLine();
                symptomNames = header.Split(',').TakeWhile(col => col != "prognosis").ToList();
            }
        }

        private void LoadDiseaseData(string filePath)
        {
            var data = mlContext.Data.LoadFromTextFile<SymptomData>(filePath, separatorChar: ',', hasHeader: true);
            diseaseData = mlContext.Data.CreateEnumerable<SymptomData>(data, reuseRowObject: false).ToList();
        }

        public string Predict(string symptomsText)
        {
            if (model == null)
            {
                throw new InvalidOperationException("The model has not been trained yet.");
            }

            var symptomVector = ConvertSymptomsToFeatures(symptomsText);

            var perfectMatch = FindPerfectMatch(symptomVector);
            if (!string.IsNullOrEmpty(perfectMatch))
            {
                return $"Perfect match found! Predicted Disease: {perfectMatch}. However, you should go to a doctor for specific details.";
            }

            var predictionEngine = mlContext.Model.CreatePredictionEngine<SymptomData, SymptomPrediction>(model);
            var input = new SymptomData { Symptoms = symptomVector };
            var prediction = predictionEngine.Predict(input);

            var matchingDiseases = GetMatchingDiseases(symptomsText, true);
            if (matchingDiseases.Any())
            {
                string diseases = string.Join(", ", matchingDiseases);
                return $"No exact match found. Possible diseases based on symptoms are: {diseases}. Please consult a doctor for specific details.";
            }

            return "No diseases could be identified based on the given symptoms.";
        }

        private string FindPerfectMatch(float[] inputVector)
        {
            foreach (var row in diseaseData)
            {
                if (VectorsAreEqual(inputVector, row.Symptoms))
                {
                    return row.Disease;
                }
            }
            return null;
        }

        private bool VectorsAreEqual(float[] vector1, float[] vector2)
        {
            if (vector1.Length != vector2.Length)
            {
                return false;
            }

            for (int i = 0; i < vector1.Length; i++)
            {
                if (vector1[i] != vector2[i])
                {
                    return false;
                }
            }

            return true;
        }

        private List<string> GetMatchingDiseases(string symptomsText, bool includeSeverity = false)
        {
            var symptomWeights = CalculateSymptomWeights(diseaseData);
            var diseaseSeverity = includeSeverity ? CalculateDiseaseSeverity(diseaseData, symptomWeights) : null;
            var inputVector = ConvertSymptomsToFeatures(symptomsText);

            var matchingDiseases = new List<string>();

            foreach (var row in diseaseData)
            {
                if (CheckSymptomMatch(inputVector, row.Symptoms))
                {
                    if (includeSeverity && diseaseSeverity != null)
                    {
                        string severity = diseaseSeverity.ContainsKey(row.Disease) ? diseaseSeverity[row.Disease] : "Unknown";
                        matchingDiseases.Add($"{row.Disease} ({severity} severity)");
                    }
                    else
                    {
                        matchingDiseases.Add(row.Disease);
                    }
                }
            }

            return matchingDiseases.Distinct().ToList();
        }

        private bool CheckSymptomMatch(float[] inputVector, float[] diseaseVector)
        {
            int commonCount = 0;
            int inputCount = inputVector.Count(x => x == 1);

            for (int i = 0; i < inputVector.Length; i++)
            {
                if (inputVector[i] == 1 && diseaseVector[i] == 1)
                {
                    commonCount++;
                }
            }

            float similarity = (float)commonCount / inputCount;
            return similarity > 0.5;
        }

        private Dictionary<string, int> CalculateSymptomWeights(IEnumerable<SymptomData> rows)
        {
            var symptomCounts = new Dictionary<string, int>();
            foreach (var row in rows)
            {
                for (int i = 0; i < row.Symptoms.Length; i++)
                {
                    if (row.Symptoms[i] == 1)
                    {
                        string symptomStr = symptomNames[i].ToLower();
                        symptomCounts[symptomStr] = symptomCounts.GetValueOrDefault(symptomStr, 0) + 1;
                    }
                }
            }

            return symptomCounts.ToDictionary(kvp => kvp.Key, kvp => (int)(100 * (1.0 / Math.Sqrt(kvp.Value + 1))));
        }

        private Dictionary<string, string> CalculateDiseaseSeverity(IEnumerable<SymptomData> rows, Dictionary<string, int> symptomWeights)
        {
            var severityDictionary = new Dictionary<string, string>();

            foreach (var row in rows)
            {
                int severityScore = 0;
                for (int i = 0; i < row.Symptoms.Length; i++)
                {
                    if (row.Symptoms[i] == 1)
                    {
                        string symptomStr = symptomNames[i].ToLower();
                        severityScore += symptomWeights.GetValueOrDefault(symptomStr, 0);
                    }
                }

                string severity = severityScore < 40 ? "High" :
                                  severityScore <= 70 ? "Medium" : "Low";

                severityDictionary[row.Disease] = severity;
            }

            return severityDictionary;
        }

        private float[] ConvertSymptomsToFeatures(string symptomsText)
        {
            float[] symptomVector = new float[symptomNames.Count];
            var inputWords = ProcessSymptomText(symptomsText);

            for (int i = 0; i < symptomNames.Count; i++)
            {
                var normalizedSymptom = symptomNames[i].Replace("_", " ").ToLower().Trim();
                if (inputWords.Contains(normalizedSymptom))
                {
                    symptomVector[i] = 1;
                }
            }
            return symptomVector;
        }

        private HashSet<string> ProcessSymptomText(string symptomsText)
        {
            return symptomsText.ToLower()
                               .Split(new char[] { ',', '.', ';', '!', '?' }, StringSplitOptions.RemoveEmptyEntries)
                               .Select(word => word.Trim())
                               .ToHashSet();
        }
    }
}
