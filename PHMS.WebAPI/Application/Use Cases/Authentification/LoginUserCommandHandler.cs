using Domain.Repositories;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Use_Cases.Authentification
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, string>
    {
        private readonly IMedicRepository medicRepository;
        private readonly IPatientRepository patientRepository;
        private readonly IAdminRepository   adminRepository;    
        public LoginUserCommandHandler(IMedicRepository medicRepository, IPatientRepository patientRepository, IAdminRepository adminRepository)
        {
            this.medicRepository = medicRepository;
            this.patientRepository = patientRepository;
            this.adminRepository = adminRepository;
        }

        public async Task<string> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            string? token = null;
            try
            {
                token = await medicRepository.Login(request.Email, request.Password);
                return token;
            }
            catch (Exception medicEx)
            {
                try
                {
                    token = await patientRepository.Login(request.Email, request.Password);
                    return token;
                }
                catch (Exception patientEx)
                {
                    try
                    {
                        token = await adminRepository.Login(request.Email, request.Password);
                        return token;
                    }
                    catch(Exception adminEx)
                    {
                        throw new Exception($"{adminEx.Message}");
                    }
                }
            }
        }
    }
}
