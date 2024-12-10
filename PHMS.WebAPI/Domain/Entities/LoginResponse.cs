namespace Domain.Entities
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public Guid Id { get; set; }
        public string Role { get; set; }
    }
}
