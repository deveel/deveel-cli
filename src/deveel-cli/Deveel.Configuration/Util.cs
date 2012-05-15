using System;

namespace Deveel.Configuration {
	class Util {
		internal static String StripLeadingHyphens(String str) {
			if (str == null)
				return null;
			if (str.StartsWith("--"))
				return str.Substring(2, str.Length - 2);
			if (str.StartsWith("-"))
				return str.Substring(1, str.Length - 1);

			return str;
		}

		internal static String StripLeadingAndTrailingQuotes(String str) {
			if (str.StartsWith("\""))
				str = str.Substring(1, str.Length - 1);
			if (str.EndsWith("\""))
				str = str.Substring(0, str.Length - 1);
			return str;
		}
	}
}