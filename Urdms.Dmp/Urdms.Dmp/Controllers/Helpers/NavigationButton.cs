namespace Urdms.Dmp.Controllers.Helpers
{
	public class NavigationButton
	{
		public string Previous { get; set; }
		public string Next { get; set; }
        public string Save { get; set; }

		public NavigationButton()
		{
			Previous = "Save and Previous";
			Next = "Save and Next";
		    Save = "Save ";
		}
	}
}