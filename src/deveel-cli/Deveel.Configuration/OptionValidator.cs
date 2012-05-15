using System;

namespace Deveel.Configuration {
	class OptionValidator {
		internal static void ValidateOption(String opt) {
			// check that opt is not NULL
			if (opt == null)
				return;
			
			// handle the single character opt
			if (opt.Length == 1) {
				char ch = opt[0];
				
				if (!IsValidOpt(ch))
					throw new ArgumentException("illegal option value '" + ch + "'");
			}
			
			// handle the multi character opt
			else {
				char[] chars = opt.ToCharArray();

				foreach(char t in chars) {
					if (!IsValidChar(t))
						throw new ArgumentException("opt contains illegal character value '" + t + "'");
				}
			}
		}
		
		private static bool IsValidOpt(char c) {
			return IsValidChar(c) || c == ' ' || c == '?' || c == '@';
		}
		
		private static bool IsValidChar(char c) {
			return Char.IsLetterOrDigit(c);
		}
	}
}