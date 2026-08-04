using System;

namespace C3.Infrastructure.Preferences
{
	[Flags]
	public enum UserPreferenceFields
	{
		None = 0x0,
		ShowMessages = 0x1,
		DefaultDirectory = 0x2,
		UpdatePolicy = 0x4,
		LastUpdateCheck = 0x8,
		All = 0xF
	}
}
