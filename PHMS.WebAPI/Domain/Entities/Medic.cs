namespace Domain.Entities
{
    public class Medic : User
    {
        public required string Rank { get; set; }
        public required string Specialization { get; set; }
        public required string Hospital { get; set; }

    }
}