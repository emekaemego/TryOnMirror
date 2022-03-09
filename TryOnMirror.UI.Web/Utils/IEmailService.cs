namespace SymaCord.TryOnMirror.UI.Web.Utils
{
    public interface IEmailService
    {
        void SendEmail(string from, string to, string subject, string message);
        void SendEmail(string from, string to, string CC, string BCC, string subject, string message);
    }
}