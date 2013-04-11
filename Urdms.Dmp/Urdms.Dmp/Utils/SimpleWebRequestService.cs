using System.IO;
using System.Net;
using System.Text;

namespace Urdms.Dmp.Utils
{
    public interface ISimpleWebRequestService
    {
        string GetResponseText(string url);
    }

    public class SimpleWebRequestService : ISimpleWebRequestService
    {
        public string GetResponseText(string url)
        {
            var request = WebRequest.Create(url);
            using(var response = request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        var text = reader.ReadToEnd();
                        return text;
                    }
                }
            }
        }
    }
}