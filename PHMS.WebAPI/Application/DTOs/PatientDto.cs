using Domain.Entities;

namespace Application.DTOs
{
    public class PatientDto: UserDto
    {
        public required List<PatientRecord> PatientRecords { get; set; }
    }
}
