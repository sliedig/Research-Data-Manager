using System.Collections.Generic;

namespace Urdms.NotificationService
{
    public interface IBaseEmail
    {
        List<string> To { get; set; }
        List<string> Cc { get; set; }
        string Subject { get; set; }
    }
}