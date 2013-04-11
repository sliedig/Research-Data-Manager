using System;
using System.ComponentModel;

namespace Urdms.DocumentBuilderService.Models.Enums
{
	[Flags]
	public enum DataOwners
	{
		None = 0,
		Researcher = 1,
		[Description("My Institution")]
		MyInstitution = 2,
		Other = 4
	}
}