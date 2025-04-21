

using Clinica_back_end.Interfaces;
namespace Clinica_back_end.services
{
    public class MailService(IMailRepository repository) : IMailService
    {
        private readonly IMailRepository _repository = repository;

        public string MakeBodyType1(string email, string fullName, string fecha, string status, string sucursal)
        {
            return _repository.MakeBodyType1(email, fullName, fecha, status, sucursal);
        }

        public void SendMail(string bodyMail, string subject, string targetMail, string senderMailDisplay)
        {
            _repository.SendMailAsync(bodyMail, subject, targetMail, senderMailDisplay);
        }
    }
}
