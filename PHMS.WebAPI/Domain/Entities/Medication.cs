namespace Domain.Entities
{
    public enum MedicationType
    {
        Tablet,
        Capsule,
        Syrup,
        Injection,
        Cream,
        Ointment,
        Drops,
        Inhaler,
        Spray,
        Patch,
        Suppository,
        Implant,
        Powder,
        Gel,
        Lotion,
        Lozenge,
        Solution,
        Suspension,
        Syringe,
        Other
    }
    public class Medication
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }

        public MedicationType Type { get; set; }
        public required List<string> Ingredients { get; set; }
        public required List<string> AdverseEffects { get; set; }
    }
}
