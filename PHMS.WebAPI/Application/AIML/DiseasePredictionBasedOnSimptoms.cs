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

            // Convertim simptomele în vector binar
            var symptomVector = ConvertSymptomsToFeatures(symptomsText);

            // Verificăm potrivirea perfectă în datele din CSV
            var exactMatchDisease = FindPerfectMatch(symptomVector);
            if (!string.IsNullOrEmpty(exactMatchDisease))
            {
                return $"Perfect match found! Predicted Disease: {exactMatchDisease}";
            }

            // Dacă nu există potriviri perfecte, creăm motorul de predicție
            var input = new SymptomData { Symptoms = symptomVector };
            var predictionEngine = mlContext.Model.CreatePredictionEngine<SymptomData, SymptomPrediction>(model);
            var prediction = predictionEngine.Predict(input);

            // Potriviri parțiale (matching diseases)
            var matchingDiseases = GetMatchingDiseases(symptomsText);

            if (matchingDiseases.Any())
            {
                string diseases = string.Join(", ", matchingDiseases);
                return $"No exact match found based on your symptoms.\nHowever, possible diseases are: {diseases}";
            }

            return "No diseases could be identified based on the given symptoms.";
        }

        // Metodă pentru găsirea potrivirii perfecte
        private string FindPerfectMatch(float[] inputVector)
        {
            // Încarcă datele din fișierul CSV
            var data = mlContext.Data.LoadFromTextFile<SymptomData>(Path.GetFullPath("symbipredict_2022.csv"), separatorChar: ',', hasHeader: true);
            var rows = mlContext.Data.CreateEnumerable<SymptomData>(data, reuseRowObject: false).ToList();

            // Căutăm un rând care are exact același vector de simptome
            foreach (var row in rows)
            {
                if (VectorsAreEqual(inputVector, row.Symptoms))
                {
                    return row.Disease; // Returnăm boala dacă există o potrivire perfectă
                }
            }

            return null; // Nu există potrivire perfectă
        }

        // Metodă pentru compararea vectorilor
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

        private (List<string> ExactMatches, List<string> PartialMatches) GetMatchingDiseasesWithDetails(string symptomsText)
        {
            var exactMatches = new List<string>();
            var partialMatches = new List<string>();

            // Încarcă datele din fișierul CSV
            var data = mlContext.Data.LoadFromTextFile<SymptomData>(Path.GetFullPath("symbipredict_2022.csv"), separatorChar: ',', hasHeader: true);
            var rows = mlContext.Data.CreateEnumerable<SymptomData>(data, reuseRowObject: false).ToList();

            // Calculăm ponderile simptomelor și severitatea bolilor
            var symptomWeights = CalculateSymptomWeights(rows);
            var diseaseSeverity = CalculateDiseaseSeverity(rows, symptomWeights);

            // Transformăm simptomele utilizatorului într-un vector binar
            var inputVector = ConvertSymptomsToFeatures(symptomsText);

            foreach (var row in rows)
            {
                // Verificăm potrivirea perfectă
                if (CheckPerfectMatch(inputVector, row.Symptoms))
                {
                    exactMatches.Add(row.Disease);
                }
                // Verificăm potrivirea parțială
                else if (CheckSymptomMatch(inputVector, row.Symptoms))
                {
                    string severity = diseaseSeverity.ContainsKey(row.Disease) ? diseaseSeverity[row.Disease] : "Unknown";
                    partialMatches.Add($"{row.Disease} ({severity} severity)");
                }
            }

            return (ExactMatches: exactMatches, PartialMatches: partialMatches.Distinct().ToList());
        }
        private bool CheckPerfectMatch(float[] inputVector, float[] diseaseVector)
        {
            // Potrivire perfectă înseamnă că toate pozițiile active în input sunt active și în vectorul bolii
            for (int i = 0; i < inputVector.Length; i++)
            {
                if (inputVector[i] == 1 && diseaseVector[i] != 1)
                {
                    return false; // Simptomul este activ în input, dar nu și în boală
                }
            }

            return true;
        }

        // Metodă ajustată pentru potrivirea parțială
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

            // Scorul de similaritate trebuie să fie mai mare decât 0.5 pentru a considera o potrivire
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



        private float[] ConvertSymptomsToFeatures(string symptomsText)
        {
            float[] symptomVector = new float[symptomNames.Count];

            // Procesăm textul și îl împărțim în cuvinte/expresii individuale
            var inputWords = symptomsText.ToLower()
                                         .Split(new char[] { ',', '.', ';', '!', '?' }, StringSplitOptions.RemoveEmptyEntries)
                                         .Select(word => word.Trim())
                                         .Distinct() // Eliminăm duplicatele
                                         .ToArray();

            // Iterăm prin lista oficială de simptome
            for (int i = 0; i < symptomNames.Count; i++)
            {
                var normalizedSymptom = symptomNames[i].Replace("_", " ").ToLower().Trim();

                // Verificăm dacă există o potrivire exactă
                if (inputWords.Contains(normalizedSymptom))
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
