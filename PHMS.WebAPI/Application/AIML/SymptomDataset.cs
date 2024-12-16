namespace Application.AIML
{
    public class SymptomDataset
    {
        public static List<SymptomData> GetTrainingData()
        {
            return new List<SymptomData>
            {
                new SymptomData { Symptoms = "febra, dureri musculare, oboseala", Disease = "Gripa" },
                new SymptomData { Symptoms = "tuse, dureri în piept, dificultăți de respirație", Disease = "Bronșită" },
                new SymptomData { Symptoms = "dureri de cap, greață, sensibilitate la lumină", Disease = "Migrenă" },
                new SymptomData { Symptoms = "durere în gât, febră, dificultăți la înghițire", Disease = "Amigdalită" },
                new SymptomData { Symptoms = "durere abdominală, greață, diaree", Disease = "Intoxicație alimentară" },
                new SymptomData { Symptoms = "mâncărimi, strănut, nas înfundat", Disease = "Alergie" },
                new SymptomData { Symptoms = "tuse, febră, dificultăți respiratorii", Disease = "COVID-19" },
                new SymptomData { Symptoms = "dureri de cap, febră, erupție pe piele", Disease = "Varicelă" }
            };
        
        }

    }
}
