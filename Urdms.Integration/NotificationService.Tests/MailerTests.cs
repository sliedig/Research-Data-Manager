using System.IO;
using NUnit.Framework;
using Urdms.NotificationService;

namespace NotificationService.Tests
{
    public class MailerShould
    {
        [Test]
        public void View_path_directory_should_exist()
        {
            Assert.That(new DirectoryInfo(new Mailer().ViewPath).Exists);
        }

        [Test]
        public void View_path_directory_should_have_views()
        {
            Assert.That(new DirectoryInfo(new Mailer().ViewPath).GetFiles("*.cshtml").Length, Is.GreaterThan(0));
        }

    }
}