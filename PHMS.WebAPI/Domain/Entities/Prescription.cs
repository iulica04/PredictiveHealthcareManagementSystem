namespace Domain.Entities
{
    public class Prescription
    {
        public Guid Id { get; set; }
        public DateTime DateIssued { get; set; }
        public required List<Medication> Medications { get; set; }
    }
}
