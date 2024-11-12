using Domain.Common;
using MediatR;

namespace Application.Commands.MedicalConditionCommands
{
    public class UpdateMedicalConditionCommand : IRequest<Result<Unit>>
    {
        public Guid MedicalConditionId { get; set; }

        public Guid PatientId { get; set; }
        public string Name { get; set; }
        public  string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public  string CurrentStatus { get; set; }
        public Boolean IsGenetic { get; set; }
        public  string Recommendation { get; set; }
        //public List<Treatment> Treatments { get; set; }
    }
}
