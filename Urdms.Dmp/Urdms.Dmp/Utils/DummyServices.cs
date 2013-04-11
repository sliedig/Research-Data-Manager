using System;
using System.Collections.Generic;
using System.Linq;
using Curtin.Framework.Common.Auth;
using Curtin.Framework.Common.UserService;
using Urdms.Dmp.Integration.UserService;

namespace Urdms.Dmp.Utils
{
    public class DummyUserLookupService : ICurtinUserService
    {
        public const string JoeResearch = "XX12345";
        public const string QaApproverAdmin = "YY23456";
        public const string SecondaryApprover = "ZZ34567";

        public static IEnumerable<UrdmsUser> GetUsers()
        {
			yield return new UrdmsUser { CurtinId = JoeResearch, FirstName = "Joe", LastName = "Research", FullName = "Joe Research", EmailAddress = "j.research@email.edu.au" };
			yield return new UrdmsUser { CurtinId = QaApproverAdmin, FirstName = "QA", LastName = "ApproverAdmin", FullName = "QA ApproverAdmin", EmailAddress = "qa.approveradmin@email.edu.au" };
            yield return new UrdmsUser { CurtinId = SecondaryApprover, FirstName = "Secondary", LastName = "ApproverAdmin", FullName = "Secondary Approver", EmailAddress = "secondary.approver@email.edu.au" };
        }


        public static IEnumerable<string> GetRoles(string curtinId)
        {
            switch (curtinId.ToUpper())
            {
				case QaApproverAdmin:
                    yield return "QA-Approver";
                    yield return "staff";
                    break;
				case SecondaryApprover:
                    yield return "Secondary-Approver";
                    yield return "staff";
                    break;
				case JoeResearch:
                    yield return "staff";
                    break;
            }
        }

        public IStaff GetStaff(string id)
        {
            throw new NotImplementedException();
        }

        public IStudent GetStudent(string id, bool fetchCwa = false)
        {
            throw new NotImplementedException();
        }

        public ICurtinUser GetUser(string id)
        {
            return GetUsers().SingleOrDefault(o => o.CurtinId.Equals(id, StringComparison.InvariantCultureIgnoreCase));
        }

        public ICurtinPerson GetCurtinPerson(string id)
        {
            throw new NotImplementedException();
        }
    }

    public class DummyDirectoryEntryService : IDirectoryEntryService
    {
        public IEnumerable<string> GetGroupMembership(string path, string domainUserName, string password, string userName)
        {
            return DummyUserLookupService.GetRoles(domainUserName);
        }
    }

    public class DummyMembershipService : IMembershipService
    {
        public MembershipValidationResult ValidateUser(string userName, string password)
        {
            var result = new MembershipValidationResult
                             {
                                 Error = "",
                                 UserName = userName.ToUpper(),
                                 Success = false
                             };
            if (DummyUserLookupService.GetUsers().Any(o => o.CurtinId.Equals(userName, StringComparison.InvariantCultureIgnoreCase)))
            {
                result.Roles.AddRange(DummyUserLookupService.GetRoles(userName));
                result.Success = true;
            }
            else
            {
                result.Error = "Invalid Urdms user ID or password.";
            }
            return result;
        }
    }
}
