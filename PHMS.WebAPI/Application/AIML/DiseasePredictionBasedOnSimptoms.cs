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
            Console.WriteLine("Symptom names extracted:");
            Console.WriteLine(string.Join(", ", symptomNames));
        }

        public string Predict(string symptomsText)
        {
            if (model == null)
            {
                throw new InvalidOperationException("The model has not been trained yet.");
            }

            var symptomVector = ConvertSymptomsToFeatures(symptomsText);
            var input = new SymptomData { Symptoms = symptomVector };
            var predictionEngine = mlContext.Model.CreatePredictionEngine<SymptomData, SymptomPrediction>(model);
            var prediction = predictionEngine.Predict(input);
            var matchingDiseases = GetMatchingDiseases(symptomsText);
            string result = $"Predicted Disease: {prediction.PredictedDisease}\n";

            if (matchingDiseases.Any())
            {
                string diseases = string.Join(", ", matchingDiseases);
                result += $"Additionally, based on partial matches, possible diseases are: {diseases}";
            }

            return result;
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
                        if (!symptomCounts.ContainsKey(symptomStr))
                        {
                            symptomCounts[symptomStr] = 0;
                        }
                        symptomCounts[symptomStr]++;
                    }
                }
            }

            var totalRows = rows.Count();
            var symptomWeights = symptomCounts
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => (int)(100 * (1.0 / Math.Sqrt(kvp.Value + 1))) 
                );

            Console.WriteLine("Adjusted Symptom Weights:");
            foreach (var kvp in symptomWeights)
            {
                Console.WriteLine($"{kvp.Key}: {kvp.Value}");
            }

            return symptomWeights;
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
                        if (symptomWeights.TryGetValue(symptomStr, out int weight))
                        {
                            severityScore += weight;
                        }
                    }
                }

                string severity = severityScore < 40  ? "High" :  
                                  severityScore >= 40  && severityScore <=70? "Medium" : 
                                  "Low";

                severityDictionary[row.Disease] = severity;

                Console.WriteLine($"Disease: {row.Disease}, Severity Score: {severityScore}, Classified as: {severity}");
            }

            return severityDictionary;
        }

        private List<string> GetMatchingDiseases(string symptomsText)
        {
            List<string> matchingDiseases = new List<string>();

            var data = mlContext.Data.LoadFromTextFile<SymptomData>(Path.GetFullPath("symbipredict_2022.csv"), separatorChar: ',', hasHeader: true);
            var rows = mlContext.Data.CreateEnumerable<SymptomData>(data, reuseRowObject: false).ToList();
            var symptomWeights = CalculateSymptomWeights(rows);
            var diseaseSeverity = CalculateDiseaseSeverity(rows, symptomWeights);
            var inputVector = ConvertSymptomsToFeatures(symptomsText);

            foreach (var row in rows)
            {
                bool isMatching = CheckSymptomMatch(inputVector, row.Symptoms);

                if (isMatching)
                {
                    string severity = diseaseSeverity.ContainsKey(row.Disease) ? diseaseSeverity[row.Disease] : "Unknown";
                    matchingDiseases.Add($"{row.Disease} ({severity} severity)");
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

        private float[] ConvertSymptomsToFeatures(string symptomsText)
        {
            float[] symptomVector = new float[symptomNames.Count];

            var inputSymptoms = symptomsText.ToLower()
                                        .Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                        .SelectMany(s => s.Split(new string[] { "and" }, StringSplitOptions.RemoveEmptyEntries))
                                        .ToArray();

            for (int i = 0; i < symptomNames.Count; i++)
            {
                if (inputSymptoms.Contains(symptomNames[i].Replace("_", "").ToLower()))
                {
                    symptomVector[i] = 1;
                }
            }

            Console.WriteLine("Symptom vector generated:");
            Console.WriteLine(string.Join(", ", symptomVector));

            return symptomVector;
        }
    }
}
