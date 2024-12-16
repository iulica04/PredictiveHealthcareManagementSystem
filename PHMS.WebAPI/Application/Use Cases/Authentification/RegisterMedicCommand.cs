namespace Application.Use_Cases.Authentification
{
    public class RegisterMedicCommand : RegisterUserCommand
    {
        public required string Rank { get; set; }
        public required string Specialization { get; set; }
        public required string Hospital { get; set; }
    }
}
