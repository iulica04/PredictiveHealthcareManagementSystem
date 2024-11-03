namespace Domain.Entities
{
    public class Prescription
    {
        public Guid Id { get; set; }
        public DateTime DateIssued { get; set; }
        List<Medication> Medications { get; set; }
    }
}
