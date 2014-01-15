using System;
using System.Collections.Generic;

namespace Deveel.Configuration {
	public class HelpSettings {
		private IComparer<IOption> optionComparer;

		public HelpSettings() {
			Width = DefaultWidth;
			LeftPadding = DefaultLeftPad;
			DescriptionPadding = DefaultDescPad;
			SyntaxPrefix = DefaultSyntaxPrefix;
			OptionPrefix = DefaultOptPrefix;
			LongOptionPrefix = DefaultLongOptPrefix;
			ArgumentName = DefaultArgName;
			NewLine = DefaultNewLine;

			OptionComparer = DefaultOptionComparer;
		}

		public const int DefaultWidth = 74;
		public const int DefaultLeftPad = 1;
		public const int DefaultDescPad = 3;

		public const String DefaultSyntaxPrefix = "usage: ";
		public const String DefaultOptPrefix = "-";
		public const String DefaultLongOptPrefix = "--";
		public const String DefaultArgName = "arg";

		public static readonly string DefaultNewLine = Environment.NewLine;
		public static readonly IComparer<IOption> DefaultOptionComparer = new OptionComparerImpl();


		public int Width { get; set; }

		public int LeftPadding { get; set; }

		public int DescriptionPadding { get; set; }

		public string SyntaxPrefix { get; set; }

		public string NewLine { get; set; }

		public string OptionPrefix { get; set; }

		public string LongOptionPrefix { get; set; }

		public string ArgumentName { get; set; }

		public string Header { get; set; }

		public bool HasHeader {
			get { return !String.IsNullOrEmpty(Header); }
		}

		public string Footer { get; set; }

		public bool HasFooter {
			get { return !String.IsNullOrEmpty(Footer); }
		}

		public string CommandLineSyntax { get; set; }

		public IComparer<IOption> OptionComparer {
			get { return optionComparer; }
			set {
				if (value == null)
					value = DefaultOptionComparer;

				optionComparer = value;
			}
		}

		private class OptionComparerImpl : IComparer<IOption> {
			public int Compare(IOption opt1, IOption opt2) {
				return String.Compare(opt1.Key(), opt2.Key(), StringComparison.OrdinalIgnoreCase);
			}
		}
	}
}