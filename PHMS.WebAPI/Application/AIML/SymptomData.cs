using Microsoft.ML.Data;

namespace Application.AIML
{
    public class SymptomData
    {
        [LoadColumn(0, 131)] // Coloanele simptomelor (0 până la 131)
        [VectorType(132)]   // Vector de 132 elemente (simptomele)
        public float[] Symptoms { get; set; }

        [LoadColumn(132)] // Ultima coloană - "prognosis" (boala)
        public string Disease { get; set; }
    }

}
