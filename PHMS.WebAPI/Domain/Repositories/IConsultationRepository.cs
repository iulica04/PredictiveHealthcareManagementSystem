using Domain.Common;
using Domain.Entities;

namespace Domain.Repositories
{
    public interface IConsultationRepository
    {
        Task<Result<Guid>> RequestConsultation(Consultation consultation);
        //Task UpdateAsync(Medic medic);
        //Task DeleteAsync(Guid id);
    }
}
