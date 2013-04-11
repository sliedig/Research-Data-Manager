using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;

namespace Urdms.NotificationService
{
    public class BaseEmail
    {
        public List<string> To { get; set; }
        public List<string> Cc { get; set; }
        public string Subject { get; set; }
        public string WebsiteUrl { get; private set; }
        public string Website { get; private set; }

        public BaseEmail()
        {
            WebsiteUrl = ConfigurationManager.AppSettings["UrdmsWebsiteUrl"];
            Website = ConfigurationManager.AppSettings["UrdmsWebsite"];
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            var toAddresses = string.Join(", ", Array.ConvertAll(To.ToArray(), i => i.ToString(CultureInfo.InvariantCulture)));
            return string.Format("Subject: {0}; Recipients: {1};", Subject, toAddresses);
        }
    }
}