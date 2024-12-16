using System.Reflection;
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

            Console.WriteLine($"Predicted Disease: {prediction.PredictedDisease}");
            return prediction.PredictedDisease;
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

            // Setăm pozițiile corespunzătoare pe 1
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
