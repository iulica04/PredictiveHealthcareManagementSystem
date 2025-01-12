using Domain.Enums;

namespace Domain.Entities
{
    public class Treatment
    {
        public Guid TreatmentId { get; set; }
        public TreatmentType Type { get; set; }
        public required Prescription Prescription { get; set; }
        public required string Location { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime Duration { get; set; }
        public required string Frequency { get; set; }
    }
}
