namespace Domain.Entities
{
    public class Patient : User
    {
        public required List<PatientRecord> PatientRecords { get; set; }
    }
}
