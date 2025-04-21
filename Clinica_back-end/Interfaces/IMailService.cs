    namespace Clinica_back_end.Interfaces
{
    public interface IMailService
    {
        public void SendMail(string bodyMail, string subject, string targetMail, string senderMailDisplay);

        public string MakeBodyType1(string email, string fullName, string fecha, string status, string sucursal);
    }
}
