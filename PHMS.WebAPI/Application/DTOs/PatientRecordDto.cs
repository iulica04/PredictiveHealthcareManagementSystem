using Domain.Entities;

namespace Application.DTOs
{
    public class PatientRecordDto
    {
        public Guid PatientRecordId { get; set; }
        public Guid PatientId { get; set; }
        public required MedicalCondition MedicalCondition { get; set; }
        public required Treatment Treatment { get; set; }
    }
}
