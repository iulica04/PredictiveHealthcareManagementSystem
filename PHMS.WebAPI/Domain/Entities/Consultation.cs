namespace Domain.Entities
{
    public enum ConsultationStatus
    {
        Pending,
        Accepted,
        Cancelled, 
        Done,
        Declined
    }
    public class Consultation
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public Guid MedicId { get; set; }
        public ConsultationStatus Status { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; }
        public string Conclusion { get; set; }
        // + AssociatedDocuments


    }
}
