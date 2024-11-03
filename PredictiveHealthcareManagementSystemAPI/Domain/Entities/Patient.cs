namespace Domain.Entities
{
    public class Patient : User
    {
        public List<PatientRecord> PatientRecords { get; set; }
    }
}
