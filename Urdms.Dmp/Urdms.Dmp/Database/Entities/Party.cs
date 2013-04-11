namespace Urdms.Dmp.Database.Entities
{
    public class Party
    {
        public virtual int Id { get; set; }
        public virtual string UserId { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Email { get; set; }
        public virtual string Organisation { get; set; }
        public virtual string FullName { get; set; }
    }
}
