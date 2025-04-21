using Clinica_back_end.Entities;

namespace Clinica_back_end
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendConfirmationEmailAsync(string toEmail, Cita cita)
        {

        }
    }

}
