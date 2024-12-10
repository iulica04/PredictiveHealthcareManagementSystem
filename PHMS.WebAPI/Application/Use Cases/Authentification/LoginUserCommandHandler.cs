using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.Authentification
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginResponse>
    {
        private readonly IMedicRepository medicRepository;
        private readonly IPatientRepository patientRepository;
        private readonly IAdminRepository adminRepository;
        public LoginUserCommandHandler(IMedicRepository medicRepository, IPatientRepository patientRepository, IAdminRepository adminRepository)
        {
            this.medicRepository = medicRepository;
            this.patientRepository = patientRepository;
            this.adminRepository = adminRepository;
        }

        public async Task<LoginResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var token = await medicRepository.Login(request.Email, request.Password);
                return token;
            }
            catch
            {
                try
                {
                    var token = await patientRepository.Login(request.Email, request.Password);
                    return token;
                }
                catch
                {
                    try
                    {
                        var token = await adminRepository.Login(request.Email, request.Password);
                        return token;
                    }
                    catch (Exception adminEx)
                    {
                        throw new Exception($"{adminEx.Message}");
                    }
                }
            }
        }
    }
}
