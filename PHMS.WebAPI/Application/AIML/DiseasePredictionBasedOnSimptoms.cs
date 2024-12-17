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
        private List<string> symptomNames; // Lista cu simptome

        public DiseasePredictionBasedOnSymptoms()
        {
            mlContext = new MLContext();
            symptomNames = new List<string>();
        }

        public void TrainModel()
        {
            // Calea către fișierul CSV
            string filePath = Path.GetFullPath("symbipredict_2022.csv");

            // Încarcă datele din fișierul CSV
            var dataView = mlContext.Data.LoadFromTextFile<SymptomData>(filePath, separatorChar: ',', hasHeader: true);

            // Extrage numele simptomelor (coloanele) din fișierul CSV
            ExtractSymptomNames(filePath);

            // Creează pipeline-ul pentru antrenare
            var pipeline = mlContext.Transforms.Concatenate("Features", nameof(SymptomData.Symptoms))
                .Append(mlContext.Transforms.Conversion.MapValueToKey("Label", nameof(SymptomData.Disease)))
                .Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy("Label", "Features"))
                .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            // Antrenează modelul
            model = pipeline.Fit(dataView);
            Console.WriteLine("Model trained successfully.");
        }

        // Metodă pentru extragerea numelor simptomelor (coloanele CSV)
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

            // Convertim simptomele textuale într-un vector binar
            var symptomVector = ConvertSymptomsToFeatures(symptomsText);

            // Creăm inputul pentru model
            var input = new SymptomData { Symptoms = symptomVector };

            // Creăm engine-ul de predicție
            var predictionEngine = mlContext.Model.CreatePredictionEngine<SymptomData, SymptomPrediction>(model);

            // Facem predicția
            var prediction = predictionEngine.Predict(input);

            // Căutăm bolile care se potrivesc parțial cu simptomele
            var matchingDiseases = GetMatchingDiseases(symptomsText);

            // Dacă există boli care se potrivesc
            if (matchingDiseases.Any())
            {
                // Creăm mesajul pentru utilizator
                string diseases = string.Join(", ", matchingDiseases);
                return $"Based on your symptoms, the most likely diseases are: {diseases}\n\nYou should visit your doctor or a specialist for a proper diagnosis and treatment.";
            }
            else
            {
                return "No diseases match your symptoms exactly, but it is still advisable to consult a doctor.\nYou should visit your doctor or a specialist for a proper diagnosis and treatment.";
            }
        }
        private Dictionary<string, int> CalculateSymptomWeights(IEnumerable<SymptomData> rows)
        {
            var symptomCounts = new Dictionary<string, int>();

            // Calculăm frecvența fiecărui simptom
            foreach (var row in rows)
            {
                foreach (var symptom in row.Symptoms)
                {
                    string symptomStr = symptom.ToString().Trim().ToLower();
                    if (!symptomCounts.ContainsKey(symptomStr))
                    {
                        symptomCounts[symptomStr] = 0;
                    }
                    symptomCounts[symptomStr]++;
                }
            }

            // Transformăm frecvența într-o pondere inversă
            var symptomWeights = new Dictionary<string, int>();
            foreach (var symptom in symptomCounts)
            {
                // Simptome rare primesc ponderi mari, simptome comune primesc ponderi mici
                symptomWeights[symptom.Key] = Math.Max(1, 100 / symptom.Value);
            }

            Console.WriteLine("Symptom Weights:");
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

                // Calculăm scorul pentru fiecare boală bazat pe ponderile simptomelor
                foreach (var symptom in row.Symptoms)
                {
                    string symptomStr = symptom.ToString().Trim().ToLower();
                    if (symptomWeights.TryGetValue(symptomStr, out int weight))
                    {
                        severityScore += weight;
                    }
                }

                // Clasificăm bolile în funcție de scor
                string severity = "Low";
                if (severityScore > 10 && severityScore <= 20)
                    severity = "Medium";
                else if (severityScore > 20)
                    severity = "High";

                severityDictionary[row.Disease] = severity;
            }

            return severityDictionary;
        }


        public void AssignSeverityToDiseases()
{
    // Încarcă datele din fișierul CSV
    var data = mlContext.Data.LoadFromTextFile<SymptomData>(Path.GetFullPath("symbipredict_2022.csv"), separatorChar: ',', hasHeader: true);
    var rows = mlContext.Data.CreateEnumerable<SymptomData>(data, reuseRowObject: false).ToList();

    // 1. Calculăm ponderile simptomelor
    var symptomWeights = CalculateSymptomWeights(rows);

    // 2. Calculăm gravitatea bolilor
    var diseaseSeverity = CalculateDiseaseSeverity(rows, symptomWeights);

    // Afișăm rezultatele
    foreach (var kvp in diseaseSeverity)
    {
        Console.WriteLine($"Disease: {kvp.Key}, Severity: {kvp.Value}");
    }
}


        private List<string> GetMatchingDiseases(string symptomsText)
        {
            List<string> matchingDiseases = new List<string>();

            // Obținem gravitatea automată a bolilor
        

            // Încarcă datele din fișierul CSV
            var data = mlContext.Data.LoadFromTextFile<SymptomData>(Path.GetFullPath("symbipredict_2022.csv"), separatorChar: ',', hasHeader: true);
            var rows = mlContext.Data.CreateEnumerable<SymptomData>(data, reuseRowObject: false).ToList();

            var symptomWeights = CalculateSymptomWeights(rows);
            var diseaseSeverity = CalculateDiseaseSeverity(rows, symptomWeights);


            // Transformăm simptomele utilizatorului într-un vector binar
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
            // Verificăm dacă există cel puțin o poziție cu valoarea 1 în ambele vectori
            for (int i = 0; i < inputVector.Length; i++)
            {
                if (inputVector[i] == 1 && diseaseVector[i] == 1)
                {
                    return true; // Există o potrivire
                }
            }
            return false; // Nu există nicio potrivire
        }



        private float[] ConvertSymptomsToFeatures(string symptomsText)
        {
            // Inițializăm vectorul de simptome cu 0
            float[] symptomVector = new float[symptomNames.Count];

            // Preprocesăm textul primit (ex: "flu and headache")
            var inputSymptoms = symptomsText.ToLower()
                                        .Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                        .SelectMany(s => s.Split(new string[] { "and" }, StringSplitOptions.RemoveEmptyEntries))
                                        .ToArray();

            // Setăm pozițiile corespunzătoare pe 1 în vectorul simptomelor
            for (int i = 0; i < symptomNames.Count; i++)
            {
                if (inputSymptoms.Contains(symptomNames[i].Replace("_", "").ToLower()))
                {
                    symptomVector[i] = 1;
                }
            }

            Console.WriteLine("Symptom vector generated:");
            Console.WriteLine(string.Join(", ", symptomVector)); // Debugging line

            return symptomVector;
        }

    }
}
