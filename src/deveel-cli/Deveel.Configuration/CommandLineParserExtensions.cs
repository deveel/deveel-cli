using System;

namespace Deveel.Configuration {
	public static class CommandLineParserExtensions {
		public static ICommandLine Parse(this ICommandLineParser parser, string[] args) {
			return parser.Parse(args, false);
		}
	}
}