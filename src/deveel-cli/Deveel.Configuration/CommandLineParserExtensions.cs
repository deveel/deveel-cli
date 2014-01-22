using System;

namespace Deveel.Configuration {
	public static class CommandLineParserExtensions {
		public static ICommandLine Parse(this ICommandLineParser parser, Options options, string[] args) {
			return Parse(parser, options, args, false);
		}

		public static ICommandLine Parse(this ICommandLineParser parser, Options options, string[] args, bool stopAtNotOption) {
			return parser.Parse(options, args, null, stopAtNotOption);
		}
	}
}