
namespace Domain.Entities
{
    public class PacientRecord
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public Pacient Pacient { get; set; }
        public List<MedicalCondition> MedicalConditions { get; set; } = new List<MedicalCondition>();
        public List<Treatment> Treatments { get; set; } = new List<Treatment>();
        public List<Consultation> Consultations { get; set; } = new List<Consultation>();
    }
}
