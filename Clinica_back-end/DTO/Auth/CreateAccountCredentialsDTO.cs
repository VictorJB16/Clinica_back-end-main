namespace Clinica_back_end.DTO.Auth
{
    public class CreateAccountCredentialsDTO
    {
        public string Email { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
    }
}
