
namespace Domain.Entities
{
    public class PatientRecord
    {
        public Guid PatientRecordId { get; set; }
        public Guid PatientId { get; set; }

        public MedicalCondition MedicalCondition { get; set; }
        public Treatment Treatment { get; set; }
        // + AssociatedDocuments
    }
}
