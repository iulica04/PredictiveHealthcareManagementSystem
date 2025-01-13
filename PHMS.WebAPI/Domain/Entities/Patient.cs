namespace Domain.Entities
{
    public class Patient : User
    {
        public required List<MedicalCondition> MedicalConditions{ get; set; }
    }
}
