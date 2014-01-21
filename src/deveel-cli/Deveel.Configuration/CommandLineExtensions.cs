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

		public static T GetValue<T>(this ICommandLine commandLine, string optionName) {
			return GetValue<T>(commandLine, optionName, default(T));
		}

		public static T GetValue<T>(this ICommandLine commandLine, string optionName, T defaultValue) {
			var optionValue = commandLine.GetOptionValue(optionName);
			if (optionValue == null)
				return defaultValue;

			var value = optionValue.Value;
			if (String.IsNullOrEmpty(value))
				return defaultValue;

			object returnValue = value;
			if (!(returnValue is T))
				returnValue = Convert.ChangeType(returnValue, typeof(T), CultureInfo.InvariantCulture);

			return (T)returnValue;			
		}

		public static string GetString(this ICommandLine commandLine, string optionName) {
			return commandLine.GetString(optionName, null);
		}

		public static string GetString(this ICommandLine commandLine, string optionName, string defaultValue) {
			return commandLine.GetValue(optionName, defaultValue);
		}

		public static byte GetByte(this ICommandLine commandLine, string optionName) {
			return commandLine.GetByte(optionName, 0);
		}

		public static byte GetByte(this ICommandLine commandLine, string optionName, byte defaultValue) {
			return commandLine.GetValue<byte>(optionName, defaultValue);
		}

		[CLSCompliant(false)]
		public static sbyte GetSByte(this ICommandLine commandLine, string optionName) {
			return GetSByte(commandLine, optionName, 0);
		}

		[CLSCompliant(false)]
		public static sbyte GetSByte(this ICommandLine commandLine, string optionName, sbyte defaultValue) {
			return commandLine.GetValue(optionName, defaultValue);
		}

		public static short GetInt16(this ICommandLine commandLine, string optionName) {
			return GetInt16(commandLine, optionName, 0);
		}

		public static short GetInt16(this ICommandLine commandLine, string optionName, short defaultValue) {
			return commandLine.GetValue(optionName, defaultValue);
		}

		public static int GetInt32(this ICommandLine commandLine, string optionName) {
			return GetInt32(commandLine, optionName, 0);
		}

		public static int GetInt32(this ICommandLine commandLine, string optionName, int defaultValue) {
			return commandLine.GetValue<int>(optionName, defaultValue);
		}

		public static long GetInt64(this ICommandLine commandLine, string optionName) {
			return GetInt64(commandLine, optionName, 0);
		}

		public static long GetInt64(this ICommandLine commandLine, string optionName, long defaultValue) {
			return commandLine.GetValue<long>(optionName, defaultValue);
		}

		public static float GetSingle(this ICommandLine commandLine, string optionName, float defaultValue) {
			return commandLine.GetValue<float>(optionName, defaultValue);
		}

		public static float GetSingle(this ICommandLine commandLine, string optionName) {
			return GetSingle(commandLine, optionName, 0);
		}

		public static double GetDouble(this ICommandLine commandLine, string optionName) {
			return GetDouble(commandLine, optionName, 0);
		}

		public static double GetDouble(this ICommandLine commandLine, string optionName, double defaultValue) {
			return commandLine.GetValue<double>(optionName, defaultValue);
		}

		public static decimal GetDecimal(this ICommandLine commandLine, string optionName) {
			return GetDecimal(commandLine, optionName, 0);
		}

		public static decimal GetDecimal(this ICommandLine commandLine, string optionName, decimal defaultValue) {
			return commandLine.GetValue<decimal>(optionName, defaultValue);
		}

		public static DateTime GetDateTime(this ICommandLine commandLine, string optionName) {
			return GetDateTime(commandLine, optionName, DateTime.MinValue);
		}

		public static DateTime GetDateTime(this ICommandLine commandLine, string optionName, DateTime defaultValue) {
			return commandLine.GetValue(optionName, defaultValue);
		}

		public static TimeSpan GetTimeSpan(this ICommandLine commandLine, string optionName) {
			return GetTimeSpan(commandLine, optionName, TimeSpan.MinValue);
		}

		public static TimeSpan GetTimeSpan(this ICommandLine commandLine, string optionName, TimeSpan defaultValue) {
			return commandLine.GetValue(optionName, defaultValue);
		}

		public static string[] GetOptionValues(this ICommandLine commandLine, string optionName) {
			var optionValue = commandLine.GetOptionValue(optionName);
			return optionValue == null ? null : optionValue.Values.ToArray();
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