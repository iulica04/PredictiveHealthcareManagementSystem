namespace Domain.Entities
{

    public class MedicalCondition
    {
        public Guid Id { get; set; }
        public Guid PacientId { get; set; }  // Adaugă o proprietate pentru cheia externă a pacientului
        public Pacient Pacient { get; set; }
     
    }
}
