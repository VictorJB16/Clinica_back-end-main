using System.Net.Mail;
using System.Net.Mime;
using System.Net;
using System.Text;
using Clinica_back_end.Interfaces;

namespace Clinica_back_end.services
{
    public class MailRepository : IMailRepository
    {
        private readonly IConfiguration _configuration;
        public MailRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            // Verificar si los parámetros de configuración necesarios están presentes
            string[] requiredSettings = ["AppSettings:smtpServer", "AppSettings:smtpPort", "AppSettings:hasSSL", "AppSettings:smtpUser", "AppSettings:smtpPassword"];
            foreach (string setting in requiredSettings)
            {
                if (string.IsNullOrEmpty(_configuration.GetValue<string>(setting)))
                {
                    throw new Exception($"Falta el parámetro de configuración '{setting}'.");
                }
            }
        }

        public string MakeBodyType1(string email, string fullName, string fecha, string status, string sucursal)
        {
            var body = new StringBuilder();

            body.Append($"<span style='font-size:14px;'>Estimado: {fullName} <span><br/><br/>");
            body.Append("<span style='font-size:14px;'>La plataforma Administración de citas</span><br/><br/>");
            body.Append($"<span style='font-size:14px;'>Comunica que se creo una cita para usted.</span>");
            body.Append($"<span style='font-size:14px;'>{fecha}.</span>");
            body.Append($"<span style='font-size:14px;'>{sucursal}.</span>");
            body.Append($"<span style='font-size:14px;'>{status}.</span>");

            return body.ToString();
        }

        public async Task SendMailAsync(string bodyMail, string subject, string targetMail, string senderMailDisplay)
        {
            // Configurations...
            var serverSmtp = _configuration.GetValue<string>("AppSettings:smtpServer")!;
            var portSmtp = int.Parse(_configuration.GetValue<string>("AppSettings:smtpPort")!);
            var hasSSL = _configuration.GetValue<string>("AppSettings:hasSSL") == "1";

            // Credentials...
            var userSmtp = _configuration.GetValue<string>("AppSettings:smtpUser");
            var passwordSmtp = _configuration.GetValue<string>("AppSettings:smtpPassword");

            var from = new MailAddress(userSmtp!, senderMailDisplay);
            var to = new MailAddress(targetMail);

            MailMessage mail = new(from, to) { Subject = subject };

            string message = "<html><body>" + bodyMail + "</body></html>";

            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(message, null, MediaTypeNames.Text.Html);

            mail.AlternateViews.Add(htmlView);


            NetworkCredential networkCredential = new(userSmtp, passwordSmtp);

            using SmtpClient smtpServer = new(serverSmtp, portSmtp);
            smtpServer.EnableSsl = hasSSL;
            smtpServer.Credentials = networkCredential;

            try
            {
                await smtpServer.SendMailAsync(mail);
            }
            catch (Exception ex)
            {
                // Manejar la excepción de manera adecuada...
                Console.WriteLine($"Error al enviar el correo electrónico: {ex.Message}");
            }
        }

        public Task SendMailAsync(string email, string fullName, string fecha, string status, string sucursal)
        {
            throw new NotImplementedException();
        }
    }
}


