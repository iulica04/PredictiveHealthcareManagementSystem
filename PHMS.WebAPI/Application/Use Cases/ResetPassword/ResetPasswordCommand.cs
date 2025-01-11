using Domain.Common;
using MediatR;

namespace Application.Use_Cases.ResetPassword
{
    public class ResetPasswordCommand : IRequest<Result<Unit>>
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }
}
