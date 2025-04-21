namespace Clinica_back_end.Entities
{
    public class Paciente
    {
        public int PacienteId { get; set; }
        // Foreign key para relacionar con ApplicationUser
        public string ApplicationUserId { get; set; } = null!;
        public ApplicationUser ApplicationUser { get; set; } = null!;
        public ICollection<Cita> Citas { get; set; } = null!;
    }

}
