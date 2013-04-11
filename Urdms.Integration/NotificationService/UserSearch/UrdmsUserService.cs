using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

namespace Urdms.NotificationService.UserSearch
{
    public class UrdmsUserService : IUrdmsUserService
    {
        public UrdmsUser GetUser(string id)
        {
			// Add your own user service implementation here...

            return new UrdmsUser
                   	{
                   		ContactLink = "contact link",
						EmailAddress = "email.address@yourdomain.edu.au",
						FirstName = "First Name",
						FullName = "Full Name",
						LastName = "Last Name",
						Phone = "Phone",
						PreferredName = "Preferred Name",
						UserId = "XX12345",
						UserType = "UserType"
                   	};
        }
    }
}
