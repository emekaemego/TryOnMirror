using System.Net.Mail;

namespace SymaCord.TryOnMirror.UI.Web.Utils.Impl
{
    public class EmailService : IEmailService
    {
        public void SendEmail(string from, string to, string subject, string message)
        {
            MailMessage mm = new MailMessage(from, to);
            mm.Subject = subject;
            mm.Body = message;
            mm.IsBodyHtml = true;

            Send(mm);
        }

        public void SendEmail(string from, string to, string CC, string BCC, string subject, string message)
        {
            MailMessage mm = new MailMessage(from, to);

            if (!string.IsNullOrEmpty(CC))
                mm.CC.Add(CC);

            if (!string.IsNullOrEmpty(BCC))
                mm.Bcc.Add(BCC);

            mm.Subject = subject;
            mm.Body = message;
            mm.IsBodyHtml = true;

            Send(mm);
        }

        private void Send(MailMessage Message)
        {
            SmtpClient smtp = new SmtpClient();
            smtp.Send(Message);
        }
    }
}