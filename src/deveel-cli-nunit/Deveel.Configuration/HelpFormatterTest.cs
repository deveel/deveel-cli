using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using NUnit.Framework;

namespace Deveel.Configuration {
	[TestFixture]
	public class HelpFormatterTest {
		private static readonly String Eol = Environment.NewLine;

		[Test]
		public void FindWrapPos() {
			var hf = new HelpFormatter();

			String text = "This is a test.";
			//text width should be max 8; the wrap position is 7
			Assert.AreEqual(7, hf.findWrapPos(text, 8, 0), "wrap position");
			//starting from 8 must give -1 - the wrap pos is after end
			Assert.AreEqual(-1, hf.findWrapPos(text, 8, 8), "wrap position 2");
			//if there is no a good position before width to make a wrapping look for the next one
			text = "aaaa aa";
			Assert.AreEqual(4, hf.findWrapPos(text, 3, 0), "wrap position 3");
		}

		[Test]
		public void PrintWrapped() {
			var sb = new StringBuilder();
			var hf = new HelpFormatter();

			String text = "This is a test.";

			var settings = new HelpSettings();

			String expected = "This is a" + settings.NewLine + "test.";
			hf.RenderWrappedText(settings, sb, 12, 0, text);
			Assert.AreEqual(expected, sb.ToString(), "single line text");

			sb.Length = 0;
			expected = "This is a" + settings.NewLine + "    test.";
			hf.RenderWrappedText(settings, sb, 12, 4, text);
			Assert.AreEqual(expected, sb.ToString(), "single line padded text");

			text = "  -p,--period <PERIOD>  PERIOD is time duration of form " +
			       "DATE[-DATE] where DATE has form YYYY[MM[DD]]";

			sb.Length = 0;
			expected = "  -p,--period <PERIOD>  PERIOD is time duration of" +
			           settings.NewLine +
			           "                        form DATE[-DATE] where DATE" +
			           settings.NewLine +
			           "                        has form YYYY[MM[DD]]";
			hf.RenderWrappedText(settings, sb, 53, 24, text);
			Assert.AreEqual(expected, sb.ToString(), "single line padded text 2");

			text = "aaaa aaaa aaaa" + settings.NewLine +
			       "aaaaaa" + settings.NewLine +
			       "aaaaa";

			expected = text;
			sb.Length = 0;
			hf.RenderWrappedText(settings, sb, 16, 0, text);
			Assert.AreEqual(expected, sb.ToString(), "multi line text");

			expected = "aaaa aaaa aaaa" + settings.NewLine +
			           "    aaaaaa" + settings.NewLine +
			           "    aaaaa";
			sb.Length = 0;
			hf.RenderWrappedText(settings, sb, 16, 4, text);
			Assert.AreEqual(expected, sb.ToString(), "multi-line padded text");
		}

		[Test]
		public void PrintOptions() {
			var sb = new StringBuilder();
			var hf = new HelpFormatter();
			const int leftPad = 1;
			const int descPad = 3;
			String lpad = hf.createPadding(leftPad);
			String dpad = hf.createPadding(descPad);

			var settings = new HelpSettings();

			Options options = new Options().AddOption("a", false, "aaaa aaaa aaaa aaaa aaaa");
			string expected = lpad + "-a" + dpad + "aaaa aaaa aaaa aaaa aaaa";
			hf.RenderOptions(settings, sb, 60, options, leftPad, descPad);
			Assert.AreEqual(expected, sb.ToString(), "simple non-wrapped option");

			int nextLineTabStop = leftPad + descPad + "-a".Length;
			expected = lpad + "-a" + dpad + "aaaa aaaa aaaa" + settings.NewLine +
			           hf.createPadding(nextLineTabStop) + "aaaa aaaa";
			sb.Length = 0;
			hf.RenderOptions(settings, sb, nextLineTabStop + 17, options, leftPad, descPad);
			Assert.AreEqual(expected, sb.ToString(), "simple wrapped option");


			options = new Options().AddOption("a", "aaa", false, "dddd dddd dddd dddd");
			expected = lpad + "-a,--aaa" + dpad + "dddd dddd dddd dddd";
			sb.Length = 0;
			hf.RenderOptions(settings, sb, 60, options, leftPad, descPad);
			Assert.AreEqual(expected, sb.ToString(), "long non-wrapped option");

			nextLineTabStop = leftPad + descPad + "-a,--aaa".Length;
			expected = lpad + "-a,--aaa" + dpad + "dddd dddd" + settings.NewLine +
			           hf.createPadding(nextLineTabStop) + "dddd dddd";
			sb.Length = 0;
			hf.RenderOptions(settings, sb, 25, options, leftPad, descPad);
			Assert.AreEqual(expected, sb.ToString(), "long wrapped option");

			options = new Options().
				AddOption("a", "aaa", false, "dddd dddd dddd dddd").
				AddOption("b", false, "feeee eeee eeee eeee");
			expected = lpad + "-a,--aaa" + dpad + "dddd dddd" + settings.NewLine +
			           hf.createPadding(nextLineTabStop) + "dddd dddd" + settings.NewLine +
			           lpad + "-b      " + dpad + "feeee eeee" + settings.NewLine +
			           hf.createPadding(nextLineTabStop) + "eeee eeee";
			sb.Length = 0;
			hf.RenderOptions(settings, sb, 25, options, leftPad, descPad);
			Assert.AreEqual(expected, sb.ToString(), "multiple wrapped options");
		}

		[Test]
		public void PrintHelpWithEmptySyntax() {
			var formatter = new HelpFormatter();
			Assert.Throws<InvalidOperationException>(() => formatter.PrintHelp(new Options(), new HelpSettings(), Console.Out, false),
				"null command line syntax should be rejected");
			Assert.Throws<InvalidOperationException>(
				() => formatter.PrintHelp(new Options(), new HelpSettings {CommandLineSyntax = String.Empty}, Console.Out, false),
				"empty command line syntax should be rejected");
		}

		[Test]
		public void AutomaticUsage() {
			var hf = new HelpFormatter();
			String expected = "usage: app [-a]";
			var output = new MemoryStream();
			var pw = new StreamWriter(output);

			Options options = new Options().AddOption("a", false, "aaaa aaaa aaaa aaaa aaaa");
			hf.PrintUsage(options, new HelpSettings{Width = 60, CommandLineSyntax = "app"}, pw);
			pw.Flush();
			Assert.AreEqual(expected, Encoding.UTF8.GetString(output.ToArray()).Trim(), "simple auto usage");

			output = new MemoryStream();
			pw = new StreamWriter(output);
			expected = "usage: app [-a] [-b]";
			options = new Options().AddOption("a", false, "aaaa aaaa aaaa aaaa aaaa").AddOption("b", false, "bbb");
			hf.PrintUsage(options, new HelpSettings {CommandLineSyntax = "app", Width = 60},  pw);
			pw.Flush();
			Assert.AreEqual(expected, Encoding.UTF8.GetString(output.ToArray()).Trim(), "simple auto usage");
		}

		// This test ensures the options are properly sorted
		[Test]
		public void PrintUsage() {
			var optionA = new Option("a", "first");
			var optionB = new Option("b", "second");
			var optionC = new Option("c", "third");
			var opts = new Options();
			opts.AddOption(optionA);
			opts.AddOption(optionB);
			opts.AddOption(optionC);
			var helpFormatter = new HelpFormatter();
			var bytesOut = new MemoryStream();
			var printWriter = new StreamWriter(bytesOut);
			helpFormatter.PrintUsage(opts, new HelpSettings { Width = 80, CommandLineSyntax = "app"},  printWriter);
			printWriter.Close();
			Assert.AreEqual("usage: app [-a] [-b] [-c]" + Eol, Encoding.UTF8.GetString(bytesOut.ToArray()));
		}

		[Test]
		public void PrintSortedUsage() {
			var opts = new Options();
			opts.AddOption(new Option("a", "first"));
			opts.AddOption(new Option("b", "second"));
			opts.AddOption(new Option("c", "third"));

			var helpFormatter = new HelpFormatter();

			var output = new StringWriter();
			helpFormatter.PrintUsage(opts, new HelpSettings { Width = 80, CommandLineSyntax = "app", OptionComparer = new SorterComparer()},  output);

			Assert.AreEqual("usage: app [-c] [-b] [-a]" + Eol, output.ToString());
		}

		private class SorterComparer : IComparer<IOption> {
			public int Compare(IOption o1, IOption o2) {
				// reverses the fuctionality of the default comparator
				return String.Compare(o2.Key(), o1.Key(), StringComparison.OrdinalIgnoreCase);
			}
		}

		[Test]
		public void PrintSortedUsageWithNullComparer() {
			Options opts = new Options();
			opts.AddOption(new Option("a", "first"));
			opts.AddOption(new Option("b", "second"));
			opts.AddOption(new Option("c", "third"));

			HelpFormatter helpFormatter = new HelpFormatter();

			StringWriter output = new StringWriter();
			helpFormatter.PrintUsage(opts, new HelpSettings { Width = 80, CommandLineSyntax = "app", OptionComparer = null }, output);

			Assert.AreEqual("usage: app [-a] [-b] [-c]" + Eol, output.ToString());
		}

		[Test]
		public void PrintOptionGroupUsage() {
			OptionGroup group = new OptionGroup();
			group.AddOption(OptionBuilder.New().Create("a"));
			group.AddOption(OptionBuilder.New().Create("b"));
			group.AddOption(OptionBuilder.New().Create("c"));

			Options options = new Options();
			options.AddOptionGroup(group);

			StringWriter output = new StringWriter();

			HelpFormatter formatter = new HelpFormatter();
			formatter.PrintUsage(options, new HelpSettings { Width = 80, CommandLineSyntax = "app" }, output);

			Assert.AreEqual("usage: app [-a | -b | -c]" + Eol, output.ToString());
		}

		[Test]
		public void PrintRequiredOptionGroupUsage() {
			OptionGroup group = new OptionGroup();
			group.AddOption(OptionBuilder.New().Create("a"));
			group.AddOption(OptionBuilder.New().Create("b"));
			group.AddOption(OptionBuilder.New().Create("c"));
			group.IsRequired = true;

			Options options = new Options();
			options.AddOptionGroup(group);

			StringWriter output = new StringWriter();

			HelpFormatter formatter = new HelpFormatter();
			formatter.PrintUsage(options, new HelpSettings {CommandLineSyntax = "app", Width = 80},  output);

			Assert.AreEqual("usage: app -a | -b | -c" + Eol, output.ToString());
		}

		[Test]
		public void PrintOptionWithEmptyArgNameUsage() {
			Option option = new Option("f", true, null);
			option.ArgumentName = "";
			option.IsRequired = true;

			Options options = new Options();
			options.AddOption(option);

			StringWriter output = new StringWriter();

			HelpFormatter formatter = new HelpFormatter();
			formatter.PrintUsage(options, new HelpSettings {ArgumentName = null, CommandLineSyntax = "app", Width = 80},  output);

			Assert.AreEqual("usage: app -f" + Eol, output.ToString());
		}

		[Test]
		public void Rtrim() {
			HelpFormatter formatter = new HelpFormatter();

			Assert.AreEqual(null, formatter.rtrim(null));
			Assert.AreEqual("", formatter.rtrim(""));
			Assert.AreEqual("  foo", formatter.rtrim("  foo  "));
		}

		/*
		This seems to be useless...
		[Test]
		public void Accessors() {
			HelpFormatter formatter = new HelpFormatter();

			formatter.ArgumentName = "argname";
			Assert.AreEqual("argname", formatter.ArgumentName, "arg name");

			formatter.DescriptionPadding = 3;
			Assert.AreEqual(3, formatter.DescriptionPadding, "desc padding");

			formatter.LeftPadding = 7;
			Assert.AreEqual(7, formatter.LeftPadding, "left padding");

			formatter.LongOptionPrefix = "~~";
			Assert.AreEqual("~~", formatter.LongOptionPrefix, "long opt prefix");

			formatter.NewLine = "\n";
			Assert.AreEqual("\n", formatter.NewLine, "new line");

			formatter.OptionPrefix = "~";
			Assert.AreEqual("~", formatter.OptionPrefix, "opt prefix");

			formatter.SyntaxPrefix = "-> ";
			Assert.AreEqual("-> ", formatter.SyntaxPrefix, "syntax prefix");

			formatter.Width = 80;
			Assert.AreEqual(80, formatter.Width, "width");
		}
		*/

		[Test]
		public void HeaderStartingWithLineSeparator() {
			// related to Bugzilla #21215
			Options options = new Options();
			HelpFormatter formatter = new HelpFormatter();
			String header = Eol + "Header";
			String footer = "Footer";
			StringWriter output = new StringWriter();

			var settings = new HelpSettings {
				Width = 80,
				Header = header,
				Footer = footer,
				LeftPadding = 2,
				DescriptionPadding = 2,
				CommandLineSyntax = "foobar"
			};

			formatter.PrintHelp(options, settings, output, true);
			Assert.AreEqual(
				"usage: foobar" + Eol +
				"" + Eol +
				"Header" + Eol +
				"" + Eol +
				"Footer" + Eol
				, output.ToString());
		}

		[Test]
		public void OptionWithoutShortFormat() {
			// related to Bugzilla #19383 (CLI-67)
			Options options = new Options();
			options.AddOption(new Option("a", "aaa", false, "aaaaaaa"));
			options.AddOption(new Option(null, "bbb", false, "bbbbbbb"));
			options.AddOption(new Option("c", null, false, "ccccccc"));

			HelpFormatter formatter = new HelpFormatter();
			StringWriter output = new StringWriter();

			var settings = new HelpSettings {
				Width = 80,
				LeftPadding = 2,
				DescriptionPadding = 2,
				CommandLineSyntax = "foobar"
			};

			formatter.PrintHelp(options, settings, output, true);
			Assert.AreEqual(
				"usage: foobar [-a] [--bbb] [-c]" + Eol +
				"  -a,--aaa  aaaaaaa" + Eol +
				"     --bbb  bbbbbbb" + Eol +
				"  -c        ccccccc" + Eol
				, output.ToString());
		}


		[Test]
		public void OptionWithoutShortFormat2() {
			// related to Bugzilla #27635 (CLI-26)
			Option help = new Option("h", "help", false, "print this message");
			Option version = new Option("v", "version", false, "print version information");
			Option newRun = new Option("n", "new", false, "Create NLT cache entries only for new items");
			Option trackerRun = new Option("t", "tracker", false, "Create NLT cache entries only for tracker items");

			IOption timeLimit = OptionBuilder.New().WithLongName("limit")
				.HasArgument()
				.WithValueSeparator()
				.WithDescription("Set time limit for execution, in mintues")
				.Create("l");

			IOption age = OptionBuilder.New().WithLongName("age")
				.HasArgument()
				.WithValueSeparator()
				.WithDescription("Age (in days) of cache item before being recomputed")
				.Create("a");

			IOption server = OptionBuilder.New().WithLongName("server")
				.HasArgument()
				.WithValueSeparator()
				.WithDescription("The NLT server address")
				.Create("s");

			IOption numResults = OptionBuilder.New().WithLongName("results")
				.HasArgument()
				.WithValueSeparator()
				.WithDescription("Number of results per item")
				.Create("r");

			IOption configFile = OptionBuilder.New().WithLongName("config")
				.HasArgument()
				.WithValueSeparator()
				.WithDescription("Use the specified configuration file")
				.Create();

			Options mOptions = new Options();
			mOptions.AddOption(help);
			mOptions.AddOption(version);
			mOptions.AddOption(newRun);
			mOptions.AddOption(trackerRun);
			mOptions.AddOption(timeLimit);
			mOptions.AddOption(age);
			mOptions.AddOption(server);
			mOptions.AddOption(numResults);
			mOptions.AddOption(configFile);

			HelpFormatter formatter = new HelpFormatter();
			
			String EOL = Environment.NewLine;
			StringWriter output = new StringWriter();

			var settings = new HelpSettings {
				ArgumentName = "arg",
				Width = 80,
				Header = "header",
				Footer = "footer",
				DescriptionPadding = 2,
				LeftPadding = 2,
				CommandLineSyntax = "commandline"
			};

			formatter.PrintHelp(mOptions, settings, output, true);
			Assert.AreEqual(
				"usage: commandline [-a <arg>] [--config <arg>] [-h] [-l <arg>] [-n] [-r <arg>]" + EOL +
				"       [-s <arg>] [-t] [-v]" + EOL +
				"header" + EOL +
				"  -a,--age <arg>      Age (in days) of cache item before being recomputed" + EOL +
				"     --config <arg>   Use the specified configuration file" + EOL +
				"  -h,--help           print this message" + EOL +
				"  -l,--limit <arg>    Set time limit for execution, in mintues" + EOL +
				"  -n,--new            Create NLT cache entries only for new items" + EOL +
				"  -r,--results <arg>  Number of results per item" + EOL +
				"  -s,--server <arg>   The NLT server address" + EOL +
				"  -t,--tracker        Create NLT cache entries only for tracker items" + EOL +
				"  -v,--version        print version information" + EOL +
				"footer" + EOL
				, output.ToString());
		}
	}
}