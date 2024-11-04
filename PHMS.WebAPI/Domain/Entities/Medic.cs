namespace Domain.Entities
{
    public class Medic : User
    {
        public Guid Id { get; set; }
        public string Rank { get; set; }
        public string Specialization { get; set; }
        public string Hospital { get; set; }

    }
}