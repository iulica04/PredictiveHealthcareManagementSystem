using Microsoft.ML.Data;

namespace Application.AIML
{
    public class SymptomPrediction
    {
        [ColumnName("PredictedLabel")]
        public string PredictedDisease { get; set; }
    }


}
