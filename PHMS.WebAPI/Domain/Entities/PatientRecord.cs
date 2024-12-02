
namespace Domain.Entities
{
    public class PatientRecord
    {
        public Guid PatientRecordId { get; set; }
        public Guid PatientId { get; set; }
        public required MedicalCondition MedicalCondition { get; set; }
        public required Treatment Treatment { get; set; }
        // + AssociatedDocuments
    }
}
