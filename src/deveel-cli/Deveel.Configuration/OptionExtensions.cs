using System;

namespace Deveel.Configuration {
	public static class OptionExtensions {
		internal static string Key(this IOption option) {
			return !String.IsNullOrEmpty(option.Name) ? option.Name : option.LongName;
		}

		public static bool HasMultipleArguments(this IOption option) {
			return option.ArgumentCount > 1 || option.ArgumentCount == Option.UnlimitedValues;
		}

		public static bool HasArgument(this IOption option) {
			return option.ArgumentCount > 0 || option.ArgumentCount == Option.UnlimitedValues;
		}

		public static bool HasArgumentName(this IOption option) {
			return !String.IsNullOrEmpty(option.ArgumentName);
		}

		public static bool HasLongName(this IOption option) {
			return !String.IsNullOrEmpty(option.LongName);
		}

		public static bool HasValueSeparator(this IOption option) {
			return option.ValueSeparator > 0;
		}

		public static bool HasOptionalArguments(this IOption option) {
			return option.ArgumentCount == Option.OptionalArguments;
		}
	}
}