using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.Authentification
{
    public class LoginMedicCommandHandler : IRequestHandler<LoginMedicCommand, string>
    {
        private readonly IMedicRepository medicRepository;

        public LoginMedicCommandHandler(IMedicRepository medicRepository)
        {
            this.medicRepository = medicRepository;
        }

        public async Task<string> Handle(LoginMedicCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var token = await medicRepository.Login(request.Email, request.Password);
                return token;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
