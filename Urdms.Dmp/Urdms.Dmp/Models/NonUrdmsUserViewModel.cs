namespace Urdms.Dmp.Models
{
	public class NonUrdmsUserViewModel
	{
		public int Id { get; set; }
		public int PartyId { get; set; }
		public string FullName { get; set; }
        public int Relationship { get; set; }
		public string Organisation { get; set; }
	}
}