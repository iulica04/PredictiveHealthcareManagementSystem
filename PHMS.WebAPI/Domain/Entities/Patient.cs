namespace Domain.Entities
{
    public class Patient : User
    {
        public ICollection<Consultation> Consultations { get; set; } = new List<Consultation>();

        public required List<PatientRecord> PatientRecords { get; set; }
    }
}
