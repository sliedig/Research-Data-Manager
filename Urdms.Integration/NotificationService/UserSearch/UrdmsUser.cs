namespace Urdms.NotificationService.UserSearch
{
    public class UrdmsUser
    {
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string PreferredName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string EmailAddress { get; set; }
        public string ContactLink { get; set; }
        public string UserType { get; set; }
        public string Phone { get; set; }
        public bool Exists { get { return !string.IsNullOrEmpty(UserId); } }
    }
}