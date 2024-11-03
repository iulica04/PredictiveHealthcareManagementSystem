namespace Domain.Entities
{

    public enum TreatmentType
    {
        Drug,
        NaturalRemedy,
        Surgery,
        Admission
    }

    public class Treatment
    {
        public Guid Id { get; set; }
        public Guid PacientId { get; set; }  // Adaugă o proprietate pentru cheia externă a înregistrării pacientului
        public Pacient Pacient { get; set; }
      
    }
}
