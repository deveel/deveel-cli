using System;
using System.IO;

namespace Deveel.Configuration {
	public class TypeHandler {
		public static Object CreateValue(string str, OptionType type) {
			if (type == OptionType.String)
				return str;
			if (type == OptionType.Object)
				return CreateValue(str);
			if (type == OptionType.Number)
				return CreateNumber(str);
			if (type == OptionType.Date)
				return CreateDateTime(str);
			if (type == OptionType.Type)
				return CreateType(str);
			if (type == OptionType.File)
				return CreateFile(str);
			if (type == OptionType.ExistingFile)
				return CreateFile(str);
			if (type == OptionType.Url)
				return CreateUri(str);

			return null;
		}

		public static object CreateValue(String typeName) {
			Type type = Type.GetType(typeName, true, true);

			if (type == null)
				throw new ParseException("Unable to find the class: " + typeName);

			object value;

			try {
				value = Activator.CreateInstance(type);
			} catch (Exception e) {
				throw new ParseException(e.GetType().Name + "; Unable to create an instance of: " + typeName);
			}

			return value;
		}

		public static object CreateNumber(String str) {
			try {
				if (str.IndexOf('.') != -1)
					return Double.Parse(str);
				return Int64.Parse(str);
			} catch (FormatException e) {
				throw new ParseException(e.Message);
			}
		}

		public static Type CreateType(String typeName) {
			try {
				return Type.GetType(typeName);
			} catch (TypeLoadException) {
				throw new ParseException("Unable to find the class: " + typeName);
			}
		}

		public static DateTime CreateDateTime(String str) {
			try {
				return DateTime.Parse(str);
			} catch (FormatException e) {
				throw new ParseException(e.Message);
			}
		}

		public static Uri CreateUri(String str) {
			try {
				return new Uri(str);
			} catch (FormatException) {
				throw new ParseException("Unable to parse the URL: " + str);
			}
		}

		public static FileInfo CreateFile(String str) {
			return new FileInfo(str);
		}
	}
}