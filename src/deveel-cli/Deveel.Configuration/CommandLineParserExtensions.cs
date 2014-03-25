using System;

namespace Deveel.Configuration {
	public static class CommandLineParserExtensions {
		public static ICommandLine Parse(this ICommandLineParser parser, Options options, string[] args) {
			return Parse(parser, options, args, false);
		}

		public static ICommandLine Parse(this ICommandLineParser parser, Options options, string[] args, bool stopAtNotOption) {
			return parser.Parse(options, args, null, stopAtNotOption);
		}

		public static ICommandLine Parse(this ICommandLineParser parser, object options, string[] args) {
			return Parse(parser, options, args, false);
		}

		public static ICommandLine Parse(this ICommandLineParser parser, object options, string[] args, bool stopAtNotOption) {
			return parser.Parse(ReflectedOptions.CreateFromObject(options), args, stopAtNotOption);
		}

		public static ICommandLine ParseConsole(this ICommandLineParser parser, Options options) {
			return ParseConsole(parser, options, false);
		}

		public static ICommandLine ParseConsole(this ICommandLineParser parser, Options options, bool stopAtNotOption) {
			return parser.Parse(options, Environment.GetCommandLineArgs(), stopAtNotOption);
		}

		public static ICommandLine ParseConsole(this ICommandLineParser parser, object options) {
			return ParseConsole(parser, options, false);
		}

		public static ICommandLine ParseConsole(this ICommandLineParser parser, object options, bool stopAtNotOption) {
			return parser.ParseConsole(ReflectedOptions.CreateFromObject(options), stopAtNotOption);
		}

		public static object ParseObject(this ICommandLineParser parser, Type type, Options options, string[] args) {
			return ParseObject(parser, type, options, args, false);
		}

		public static object ParseObject(this ICommandLineParser parser, Type type, string[] args) {
			return ParseObject(parser, type, args, false);
		}

		public static object ParseObject(this ICommandLineParser parser, Type type, string[] args, bool stopAtNotOption) {
			return ParseObject(parser, type, ReflectedOptions.CreateFromType(type), args, stopAtNotOption);
		}

		public static object ParseObject(this ICommandLineParser parser, Type type, Options options, string[] args, bool stopAtNotOption) {
			var commandLine = parser.Parse(options, args, stopAtNotOption);
			var obj = Activator.CreateInstance(type, true);
			ReflectedOptions.SetToObject(options, commandLine, obj);
			return obj;
		}

		public static T ParseObject<T>(this ICommandLineParser parser, Options options, string[] args) {
			return ParseObject<T>(parser, options, args, false);
		}

		public static T ParseObject<T>(this ICommandLineParser parser, string[] args) {
			return ParseObject<T>(parser, args, false);
		}

		public static T ParseObject<T>(this ICommandLineParser parser, string[] args, bool stopAtNotOption) {
			return ParseObject<T>(parser, ReflectedOptions.CreateFromType(typeof(T)), args, stopAtNotOption);
		}

		public static T ParseObject<T>(this ICommandLineParser parser, Options options, string[] args, bool stopAtNotOption) {
			return (T) parser.ParseObject(typeof (T), options, args, stopAtNotOption);
		}
	}
}