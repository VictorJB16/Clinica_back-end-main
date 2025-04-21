using Microsoft.AspNetCore.Identity;

namespace Clinica_back_end.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        // Relación opcional con Patient
        public Paciente? Patient { get; set; } = null!;
    }
}
