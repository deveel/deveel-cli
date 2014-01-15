using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Deveel.Configuration {
	public static class CommandLineExtensions {
		internal static IOption ResolveOption(this ICommandLine commandLine, String opt) {
			opt = Util.StripLeadingHyphens(opt);
			foreach (IOption option in commandLine.Options) {
				if (opt.Equals(option.Name))
					return option;

				if (opt.Equals(option.LongName))
					return option;

			}

			return null;
		}

		public static bool HasOption(this ICommandLine commandLine, string opt) {
			return commandLine.Options.Contains(ResolveOption(commandLine, opt));
		}

		public static bool HasOption(this ICommandLine commandLine, char opt) {
			return commandLine.HasOption(opt.ToString(CultureInfo.InvariantCulture));
		}

		public static IOptionValue GetOptionValue(this ICommandLine commandLine, char optionName) {
			return commandLine.GetOptionValue(optionName.ToString(CultureInfo.InvariantCulture));
		}

		public static string[] GetOptionValues(this ICommandLine commandLine, string optionName) {
			List<string> values = new List<string>();

			foreach (IOption option in commandLine.Options) {
				if (optionName.Equals(option.Name) ||
					optionName.Equals(option.LongName)) {
					var value = commandLine.GetOptionValue(optionName);
					values.AddRange(value.Values);
				}
			}

			return values.Count == 0 ? null : values.ToArray();
		}

		public static string[] GetOptionValues(this ICommandLine commandLine, char optionName) {
			return commandLine.GetOptionValues(optionName.ToString(CultureInfo.InvariantCulture));
		}

		public static object GetParsedOptionValue(this ICommandLine commandLine, string optionName) {
			var res = commandLine.GetOptionValue(optionName);

			IOption option = commandLine.ResolveOption(optionName);
			if (option == null)
				return null;

			OptionType type = option.Type;
			return (res == null) ? null : TypeHandler.CreateValue(res.Value, type);
		}

		public static object GetOptionObject(this ICommandLine commandLine, string optionName) {
			try {
				return commandLine.GetParsedOptionValue(optionName);
			} catch (ParseException pe) {
				Console.Error.WriteLine("Exception found converting " + optionName + " to desired type: " + pe.Message);
				return null;
			}
		}

		public static object GetOptionObject(this ICommandLine commandLine, char optionName) {
			return commandLine.GetOptionObject(optionName.ToString(CultureInfo.InvariantCulture));
		}

		public static string GetOptionValue(this ICommandLine commandLine, string optionName, string defaultValue) {
			var answer = commandLine.GetOptionValue(optionName);
			return answer == null ? defaultValue : answer.Value;
		}

		public static string GetOptionValue(this ICommandLine commandLine, char optionName, string defaultValue) {
			return commandLine.GetOptionValue(optionName.ToString(CultureInfo.InvariantCulture), defaultValue);
		}

		public static IDictionary<string, string> GetOptionProperties(this ICommandLine commandLine, string opt) {
			IDictionary<string, string> props = new Dictionary<string, string>();

			foreach (IOption option in commandLine.Options) {
				if (opt.Equals(option.Name) ||
					opt.Equals(option.LongName)) {
					var value = commandLine.GetOptionValue(opt);

					IList<string> values = value.Values.ToList();
					if (values.Count >= 2) {
						// use the first 2 arguments as the key/value pair
						props[values[0]] = values[1];
					} else if (values.Count == 1) {
						// no explicit value, handle it as a bool
						props[values[0]] = "true";
					}
				}
			}

			return props;
		}
	}
}