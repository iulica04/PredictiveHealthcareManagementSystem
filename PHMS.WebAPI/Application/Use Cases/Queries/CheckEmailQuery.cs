using MediatR;

namespace Application.Use_Cases.Queries
{
    public class CheckEmailQuery : IRequest<bool>
    {
        public string Email { get; set; } = string.Empty;
    }
}
