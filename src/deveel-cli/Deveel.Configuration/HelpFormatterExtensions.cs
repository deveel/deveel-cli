using System;
using System.IO;

namespace Deveel.Configuration {
	public static class HelpFormatterExtensions {
		public static void PrintHelp(this IHelpFormatter formatter, Options options, TextWriter writer) {
			PrintHelp(formatter, options, writer, false);
		}

		public static void PrintHelp(this IHelpFormatter formatter, Options options, TextWriter writer, bool autoUsage) {
			formatter.PrintHelp(options, new HelpSettings(), writer, autoUsage);
		}

		public static void PrintHelpToConsole(this IHelpFormatter formatter, Options options) {
			PrintHelpToConsole(formatter, options, false);
		}

		public static void PrintHelpToConsole(this IHelpFormatter formatter, Options options, bool autoUsage) {
			formatter.PrintHelp(options, Console.Out, autoUsage);
		}
	}
}
