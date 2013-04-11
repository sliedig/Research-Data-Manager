using System;
using System.ComponentModel;

namespace Urdms.DocumentBuilderService.Models.Enums
{
	[Flags]
	public enum InstitutionalStorageTypes
	{
		[Description("Project Storage Space")]
		ProjectStorageSpace = 0,
		[Description("Personal Network Drive")]
		PersonalNetworkDrive = 1,
		[Description("Shared Network Drive")]
		SharedNetworkDrive = 2,
		[Description("Others")]
		Other = 4

	}
}
