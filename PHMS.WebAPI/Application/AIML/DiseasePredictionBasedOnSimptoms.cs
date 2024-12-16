using Application.AIML;
using Microsoft.ML;

public class DiseasePredictionBasedOnSimptoms
{
    private readonly MLContext mlContext;
    private ITransformer model;

    public DiseasePredictionBasedOnSimptoms()
    {
        mlContext = new MLContext();
    }

    public void TrainModel()
    {
        var trainingData = SymptomDataset.GetTrainingData();
        var dataView = mlContext.Data.LoadFromEnumerable(trainingData);

        // Log pentru verificarea datelor de antrenament
        Console.WriteLine("Training data loaded: ");
        foreach (var data in trainingData)
        {
            Console.WriteLine($"Symptom: {data.Symptoms}, Disease: {data.Disease}");
        }

        // Creează pipeline-ul pentru antrenare
        var pipeline = mlContext.Transforms.Text.FeaturizeText("Features", nameof(SymptomData.Symptoms))
            .Append(mlContext.Transforms.Conversion.MapValueToKey("Label", nameof(SymptomData.Disease))) // Map disease name to key
            .Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy("Label", "Features")) // SDCA for multiclass
            .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel", "PredictedLabel")); // Map back to original label

        model = pipeline.Fit(dataView);
        Console.WriteLine("Model trained successfully.");
    }

    public static string PreprocessSymptoms(string symptoms)
    {
        symptoms = symptoms.ToLower();
        symptoms = symptoms.Replace(",", "").Replace(".", "").Replace("!", "").Replace("?", "").Replace(";", "").Replace(":", "");
        symptoms = symptoms.Replace("si", "").Replace("sau", "").Replace("dar", "");
        return symptoms.Trim();
    }

    public string Predict(string symptoms)
    {
        if (model == null)
        {
            throw new InvalidOperationException("Modelul nu a fost încă antrenat.");
        }

        // Procesarea simptomelor înainte de a le trimite la predicție
        symptoms = PreprocessSymptoms(symptoms);

        // Transformarea simptomelor în caracteristici (features)
        var input = new SymptomData { Symptoms = symptoms };

        // Creează engine-ul de predicție
        var predictionEngine = mlContext.Model.CreatePredictionEngine<SymptomData, SymptomPrediction>(model);

        // Face predicția
        var prediction = predictionEngine.Predict(input);

        // Log pentru simptomele procesate
        Console.WriteLine($"Simptomele procesate pentru predicție: {input.Symptoms}");

        // Log pentru eticheta predicției (predicted disease name)
        Console.WriteLine($"Predicted Disease: {prediction.PredictedDisease}");

        return prediction.PredictedDisease;
    }
}
