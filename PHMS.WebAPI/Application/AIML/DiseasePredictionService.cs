namespace Application.AIML
{
    public class DiseasePredictionService
    {
        private readonly DiseasePredictionBasedOnSimptoms diseasePrediction;

        public DiseasePredictionService()
        {
            diseasePrediction = new DiseasePredictionBasedOnSimptoms();
            diseasePrediction.TrainModel(); // Train the model when the service is created
        }

        // Method that predicts the disease based on symptoms
        public string PredictDisease(string symptoms)
        {
            return diseasePrediction.Predict(symptoms);
        }
    }

}
