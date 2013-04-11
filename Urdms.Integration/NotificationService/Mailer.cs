using System.Configuration;
using System.IO;
using System.Reflection;
using ActionMailer.Net.Standalone;

namespace Urdms.NotificationService
{
    public class Mailer : RazorMailerBase, IMailer
    {
        private readonly string _fromAddress;

        public Mailer()
        {
            _fromAddress = ConfigurationManager.AppSettings["FromEmail"];
        }

        public override string ViewPath
        {
            get
            {
                return Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Replace("file:\\", string.Empty), 
                    "EmailTemplates"); 
            }
        }

        public void SendEmail<T>(T email, string template) where T : BaseEmail
        {
            email.To.ForEach(m => To.Add(m));

            if (email.Cc != null)
                email.Cc.ForEach(e => CC.Add(e));

            From = _fromAddress;
            Subject = email.Subject;

            Email(template, email).Deliver();
        }
    }
}