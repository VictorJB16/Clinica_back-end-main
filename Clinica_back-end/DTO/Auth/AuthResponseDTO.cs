namespace Clinica_back_end.DTO.Auth
{
    public class AuthResponseDTO
    {
        public string Token { get; set; } = null!;
        public DateTime Expiracion { get; set; }
    }
}
