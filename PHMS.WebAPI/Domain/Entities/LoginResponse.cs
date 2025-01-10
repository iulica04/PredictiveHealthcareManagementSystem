using Domain.Enums;

namespace Domain.Entities
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public Guid Id { get; set; }
        public UserType Role { get; set; }
    }
}
