namespace Urdms.NotificationService
{
    public interface IMailer
    {
        void SendEmail<T>(T email, string template) where T : BaseEmail;
    }
}