using Curtin.Framework.Common.Extensions;
using Curtin.Framework.Common.UserService;

namespace Urdms.Dmp.Integration.UserService
{
	public class UrdmsUser : ICurtinUser
	{
		private string _fullName;
		public string FullName
		{
			get
			{
				return _fullName ?? string.Format("{0} {1}", FirstName, LastName);
			}
			set
			{
				_fullName = value;
			}
		}

		public string CurtinId { get; set; }

		public string FirstName { get; set; }

		public string PreferredName { get; set; }

		public string LastName { get; set; }

		public string EmailAddress { get; set; }

		public string ContactLink { get; set; }

		public string UserType { get; set; }

		public string Phone { get; set; }

		public bool Exists
		{
			get { return !string.IsNullOrEmpty(CurtinId); }
		}

		public UrdmsUser()
		{
		}

		public UrdmsUser(ICurtinPerson person)
		{
		  CurtinId = person.CurtinId.ToUpper();
		  FirstName = person.FirstName;
		  PreferredName = person.PreferredName;
		  LastName = person.LastName;
		  FullName = (person.PreferredName ?? person.FirstName) + " " + person.LastName;
		  EmailAddress = person.EmailAddress;
		  ContactLink = string.Format("<a href=\"mailto:{0}\">{1}</a>", StringExtensions.AsciiEncode(EmailAddress), FullName);
		  UserType = person.UserType;
		  Phone = person.ContactPhone;
		}
	}
}