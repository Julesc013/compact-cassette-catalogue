using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Runtime.CompilerServices;

namespace C3.Infrastructure.Updates
{
	public sealed class SemanticVersion : IComparable<SemanticVersion>
	{
		public const int MaximumTextCharacters = 128;

		private readonly string[] _coreIdentifiers;

		private readonly string[] _prereleaseIdentifiers;
		public string OriginalText
		{
			get;
		}

		public string ReleaseLabel
		{
			get;
		}

		public string CoreVersion => string.Join(".", _coreIdentifiers);

		public bool HasPrerelease => _prereleaseIdentifiers.Length > 0;

		public string PrereleaseLabel => string.Join(".", _prereleaseIdentifiers);

		public string FirstPrereleaseIdentifier
		{
			get
			{
				if (_prereleaseIdentifiers.Length == 0)
				{
					return null;
				}
				return _prereleaseIdentifiers[0];
			}
		}

		private SemanticVersion(string originalText, string releaseLabel, string[] coreIdentifiers, string[] prereleaseIdentifiers)
		{
			OriginalText = originalText;
			ReleaseLabel = releaseLabel;
			_coreIdentifiers = coreIdentifiers;
			_prereleaseIdentifiers = prereleaseIdentifiers;
		}

		public static bool TryParse(string value, ref SemanticVersion parsed)
		{
			parsed = null;
			checked
			{
				if (value != null && value.Length != 0 && value.Length <= 128 && Operators.CompareString(value, value.Trim(), false) == 0)
				{
					int num = value.IndexOf('+');
					if (num >= 0 && value.IndexOf('+', num + 1) >= 0)
					{
						return false;
					}
					string text = (num >= 0) ? value.Substring(0, num) : value;
					if (num >= 0 && !AreValidIdentifiers(value.Substring(num + 1), false))
					{
						return false;
					}
					int num2 = text.IndexOf('-');
					string obj = (num2 >= 0) ? text.Substring(0, num2) : text;
					string text2 = (num2 >= 0) ? text.Substring(num2 + 1) : string.Empty;
					string[] array = obj.Split('.');
					if (array.Length != 3)
					{
						return false;
					}
					string[] array2 = array;
					for (int i = 0; i < array2.Length; i++)
					{
						if (!IsCanonicalNumericIdentifier(array2[i]))
						{
							return false;
						}
					}
					string[] prereleaseIdentifiers = new string[0];
					if (num2 >= 0)
					{
						if (!AreValidIdentifiers(text2, true))
						{
							return false;
						}
						prereleaseIdentifiers = text2.Split('.');
					}
					parsed = new SemanticVersion(value, text, array, prereleaseIdentifiers);
					return true;
				}
				return false;
			}
		}

		public int CompareTo(SemanticVersion other)
		{
			if (other == null)
			{
				return 1;
			}
			checked
			{
				int num = _coreIdentifiers.Length - 1;
				for (int i = 0; i <= num; i++)
				{
					int num2 = CompareNumericIdentifiers(_coreIdentifiers[i], other._coreIdentifiers[i]);
					if (num2 != 0)
					{
						return num2;
					}
				}
				if (!HasPrerelease && !other.HasPrerelease)
				{
					return 0;
				}
				if (!HasPrerelease)
				{
					return 1;
				}
				if (!other.HasPrerelease)
				{
					return -1;
				}
				int num3 = Math.Min(_prereleaseIdentifiers.Length, other._prereleaseIdentifiers.Length) - 1;
				for (int j = 0; j <= num3; j++)
				{
					string text = _prereleaseIdentifiers[j];
					string text2 = other._prereleaseIdentifiers[j];
					bool flag = IsNumericIdentifier(text);
					bool flag2 = IsNumericIdentifier(text2);
					int num4 = (!flag || !flag2) ? ((!flag) ? (flag2 ? 1 : string.CompareOrdinal(text, text2)) : (-1)) : CompareNumericIdentifiers(text, text2);
					if (num4 != 0)
					{
						return num4;
					}
				}
				return _prereleaseIdentifiers.Length.CompareTo(other._prereleaseIdentifiers.Length);
			}
		}

		int IComparable<SemanticVersion>.CompareTo(SemanticVersion other)
		{
			//ILSpy generated this explicit interface implementation from .override directive in CompareTo
			return this.CompareTo(other);
		}

		private static bool AreValidIdentifiers(string value, bool enforceNumericCanonicalForm)
		{
			if (value.Length == 0)
			{
				return false;
			}
			string[] array = value.Split('.');
			int num = 0;
			bool result;
			while (true)
			{
				if (num < array.Length)
				{
					string text = array[num];
					if (text.Length == 0)
					{
						result = false;
						break;
					}
					string text2 = text;
					foreach (char c in text2)
					{
						if (!IsAsciiLetterOrDigit(c) && c != '-')
						{
							return false;
						}
					}
					if (enforceNumericCanonicalForm && IsNumericIdentifier(text) && !IsCanonicalNumericIdentifier(text))
					{
						return false;
					}
					num = checked(num + 1);
					continue;
				}
				return true;
			}
			return result;
		}

		private static bool IsCanonicalNumericIdentifier(string value)
		{
			if (IsNumericIdentifier(value))
			{
				if (value.Length != 1)
				{
					return value[0] != '0';
				}
				return true;
			}
			return false;
		}

		private static bool IsNumericIdentifier(string value)
		{
			if (value.Length == 0)
			{
				return false;
			}
			foreach (char c in value)
			{
				if (c < '0' || c > '9')
				{
					return false;
				}
			}
			return true;
		}

		private static bool IsAsciiLetterOrDigit(char value)
		{
			if (value >= '0' && value <= '9')
			{
				goto IL_0024;
			}
			if (value >= 'A' && value <= 'Z')
			{
				goto IL_0024;
			}
			if (value >= 'a')
			{
				return value <= 'z';
			}
			return false;
			IL_0024:
			return true;
		}

		private static int CompareNumericIdentifiers(string left, string right)
		{
			int num = left.Length.CompareTo(right.Length);
			if (num != 0)
			{
				return num;
			}
			return string.CompareOrdinal(left, right);
		}
	}
}
