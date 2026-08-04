using Microsoft.VisualBasic.CompilerServices;
using System;

namespace C3.Infrastructure.Updates
{
	public sealed class UpdateCheckSchedule
	{
		private UpdateCheckSchedule()
		{
		}

		public static UpdateCheckPolicy Parse(string value)
		{
			UpdateCheckPolicy result = default(UpdateCheckPolicy);
			if (TryParseStored(value, ref result))
			{
				return result;
			}
			return UpdateCheckPolicy.Never;
		}

		public static bool TryParseStored(string value, ref UpdateCheckPolicy policy)
		{
			string text = (value ?? string.Empty).Trim().ToLowerInvariant();
			switch (text)
			{
			case "true":
			case "startup":
				policy = UpdateCheckPolicy.Startup;
				return true;
			case "weekly":
				policy = UpdateCheckPolicy.Weekly;
				return true;
			case "monthly":
				policy = UpdateCheckPolicy.Monthly;
				return true;
			case "false":
			case "manually":
			case "never":
				policy = UpdateCheckPolicy.Never;
				return true;
			default:
				policy = UpdateCheckPolicy.Never;
				return false;
			}
		}

		public static string Serialize(UpdateCheckPolicy value)
		{
			switch (value)
			{
			case UpdateCheckPolicy.Startup:
				return "startup";
			case UpdateCheckPolicy.Weekly:
				return "weekly";
			case UpdateCheckPolicy.Monthly:
				return "monthly";
			default:
				return "never";
			}
		}

		public static bool ShouldCheck(UpdateCheckPolicy policy, DateTime lastCheckedAt, DateTime now)
		{
			if (policy != 0 && Enum.IsDefined(typeof(UpdateCheckPolicy), policy))
			{
				DateTime dateTime = NormalizeUtc(now);
				DateTime dateTime2 = (DateTime.Compare(lastCheckedAt, DateTime.MinValue) == 0) ? DateTime.MinValue : NormalizeUtc(lastCheckedAt);
				TimeSpan timeSpan;
				if (DateTime.Compare(dateTime2, DateTime.MinValue) != 0)
				{
					timeSpan = dateTime2 - dateTime;
					if (timeSpan.TotalMinutes > 5.0)
					{
						return true;
					}
				}
				switch (policy)
				{
				case UpdateCheckPolicy.Startup:
					return true;
				case UpdateCheckPolicy.Weekly:
				{
					int result2;
					if (DateTime.Compare(dateTime2, DateTime.MinValue) != 0)
					{
						timeSpan = dateTime - dateTime2;
						result2 = ((timeSpan.TotalDays >= 7.0) ? 1 : 0);
					}
					else
					{
						result2 = 1;
					}
					return (byte)result2 != 0;
				}
				case UpdateCheckPolicy.Monthly:
				{
					int result;
					if (DateTime.Compare(dateTime2, DateTime.MinValue) != 0)
					{
						timeSpan = dateTime - dateTime2;
						result = ((timeSpan.TotalDays >= 28.0) ? 1 : 0);
					}
					else
					{
						result = 1;
					}
					return (byte)result != 0;
				}
				default:
					return false;
				}
			}
			return false;
		}

		public static DateTime NormalizeUtc(DateTime value)
		{
			if (DateTime.Compare(value, DateTime.MinValue) != 0 && value.Kind != DateTimeKind.Utc)
			{
				if (value.Kind == DateTimeKind.Unspecified)
				{
					value = DateTime.SpecifyKind(value, DateTimeKind.Local);
				}
				return value.ToUniversalTime();
			}
			return value;
		}
	}
}
