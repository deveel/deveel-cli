using System;
using System.Collections;
using System.IO;
using System.Text;

using NUnit.Framework;

namespace Deveel.Configuration {
	[TestFixture]
	public class HelpFormatterTest {
		private static readonly String EOL = Environment.NewLine;

		[Test]
		public void FindWrapPos() {
			HelpFormatter hf = new HelpFormatter();

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
			StringBuilder sb = new StringBuilder();
			HelpFormatter hf = new HelpFormatter();

			String text = "This is a test.";

			String expected = "This is a" + hf.NewLine + "test.";
			hf.renderWrappedText(sb, 12, 0, text);
			Assert.AreEqual(expected, sb.ToString(), "single line text");

			sb.Length = 0;
			expected = "This is a" + hf.NewLine + "    test.";
			hf.renderWrappedText(sb, 12, 4, text);
			Assert.AreEqual(expected, sb.ToString(), "single line padded text");

			text = "  -p,--period <PERIOD>  PERIOD is time duration of form " +
			       "DATE[-DATE] where DATE has form YYYY[MM[DD]]";

			sb.Length = 0;
			expected = "  -p,--period <PERIOD>  PERIOD is time duration of" +
			           hf.NewLine +
			           "                        form DATE[-DATE] where DATE" +
			           hf.NewLine +
			           "                        has form YYYY[MM[DD]]";
			hf.renderWrappedText(sb, 53, 24, text);
			Assert.AreEqual(expected, sb.ToString(), "single line padded text 2");

			text = "aaaa aaaa aaaa" + hf.NewLine +
			       "aaaaaa" + hf.NewLine +
			       "aaaaa";

			expected = text;
			sb.Length = 0;
			hf.renderWrappedText(sb, 16, 0, text);
			Assert.AreEqual(expected, sb.ToString(), "multi line text");

			expected = "aaaa aaaa aaaa" + hf.NewLine +
			           "    aaaaaa" + hf.NewLine +
			           "    aaaaa";
			sb.Length = 0;
			hf.renderWrappedText(sb, 16, 4, text);
			Assert.AreEqual(expected, sb.ToString(), "multi-line padded text");
		}

		[Test]
		public void PrintOptions() {
			StringBuilder sb = new StringBuilder();
			HelpFormatter hf = new HelpFormatter();
			int leftPad = 1;
			int descPad = 3;
			String lpad = hf.createPadding(leftPad);
			String dpad = hf.createPadding(descPad);
			Options options = null;
			String expected = null;

			options = new Options().AddOption("a", false, "aaaa aaaa aaaa aaaa aaaa");
			expected = lpad + "-a" + dpad + "aaaa aaaa aaaa aaaa aaaa";
			hf.renderOptions(sb, 60, options, leftPad, descPad);
			Assert.AreEqual(expected, sb.ToString(), "simple non-wrapped option");

			int nextLineTabStop = leftPad + descPad + "-a".Length;
			expected = lpad + "-a" + dpad + "aaaa aaaa aaaa" + hf.NewLine +
			           hf.createPadding(nextLineTabStop) + "aaaa aaaa";
			sb.Length = 0;
			hf.renderOptions(sb, nextLineTabStop + 17, options, leftPad, descPad);
			Assert.AreEqual(expected, sb.ToString(), "simple wrapped option");


			options = new Options().AddOption("a", "aaa", false, "dddd dddd dddd dddd");
			expected = lpad + "-a,--aaa" + dpad + "dddd dddd dddd dddd";
			sb.Length = 0;
			hf.renderOptions(sb, 60, options, leftPad, descPad);
			Assert.AreEqual(expected, sb.ToString(), "long non-wrapped option");

			nextLineTabStop = leftPad + descPad + "-a,--aaa".Length;
			expected = lpad + "-a,--aaa" + dpad + "dddd dddd" + hf.NewLine +
			           hf.createPadding(nextLineTabStop) + "dddd dddd";
			sb.Length = 0;
			hf.renderOptions(sb, 25, options, leftPad, descPad);
			Assert.AreEqual(expected, sb.ToString(), "long wrapped option");

			options = new Options().
				AddOption("a", "aaa", false, "dddd dddd dddd dddd").
				AddOption("b", false, "feeee eeee eeee eeee");
			expected = lpad + "-a,--aaa" + dpad + "dddd dddd" + hf.NewLine +
			           hf.createPadding(nextLineTabStop) + "dddd dddd" + hf.NewLine +
			           lpad + "-b      " + dpad + "feeee eeee" + hf.NewLine +
			           hf.createPadding(nextLineTabStop) + "eeee eeee";
			sb.Length = 0;
			hf.renderOptions(sb, 25, options, leftPad, descPad);
			Assert.AreEqual(expected, sb.ToString(), "multiple wrapped options");
		}

		[Test]
		public void PrintHelpWithEmptySyntax() {
			HelpFormatter formatter = new HelpFormatter();
			try {
				formatter.Options = new Options();
				formatter.PrintHelp();
				Assert.Fail("null command line syntax should be rejected");
			} catch (InvalidOperationException e) {
				// expected
			}

			try {
				formatter.Options = new Options();
				formatter.CommandLineSyntax = String.Empty;
				formatter.PrintHelp();
				Assert.Fail("empty command line syntax should be rejected");
			} catch (InvalidOperationException e) {
				// expected
			}
		}

		[Test]
		public void AutomaticUsage() {
			HelpFormatter hf = new HelpFormatter();
			Options options = null;
			String expected = "usage: app [-a]";
			MemoryStream output = new MemoryStream();
			StreamWriter pw = new StreamWriter(output);

			options = new Options().AddOption("a", false, "aaaa aaaa aaaa aaaa aaaa");
			hf.Options = options;
			hf.Width = 60;
			hf.CommandLineSyntax = "app";
			hf.PrintUsage(pw);
			pw.Flush();
			Assert.AreEqual(expected, Encoding.UTF8.GetString(output.ToArray()).Trim(), "simple auto usage");

			output = new MemoryStream();
			pw = new StreamWriter(output);
			expected = "usage: app [-a] [-b]";
			options = new Options().AddOption("a", false, "aaaa aaaa aaaa aaaa aaaa").AddOption("b", false, "bbb");
			hf.Options = options;
			hf.CommandLineSyntax = "app";
			hf.Width = 60;
			hf.PrintUsage(pw);
			pw.Flush();
			Assert.AreEqual(expected, Encoding.UTF8.GetString(output.ToArray()).Trim(), "simple auto usage");
		}

		// This test ensures the options are properly sorted
		[Test]
		public void PrintUsage() {
			Option optionA = new Option("a", "first");
			Option optionB = new Option("b", "second");
			Option optionC = new Option("c", "third");
			Options opts = new Options();
			opts.AddOption(optionA);
			opts.AddOption(optionB);
			opts.AddOption(optionC);
			HelpFormatter helpFormatter = new HelpFormatter();
			MemoryStream bytesOut = new MemoryStream();
			StreamWriter printWriter = new StreamWriter(bytesOut);
			helpFormatter.Options = opts;
			helpFormatter.Width = 80;
			helpFormatter.CommandLineSyntax = "app";
			helpFormatter.PrintUsage(printWriter);
			printWriter.Close();
			Assert.AreEqual("usage: app [-a] [-b] [-c]" + EOL, Encoding.UTF8.GetString(bytesOut.ToArray()));
		}

		[Test]
		public void PrintSortedUsage() {
			Options opts = new Options();
			opts.AddOption(new Option("a", "first"));
			opts.AddOption(new Option("b", "second"));
			opts.AddOption(new Option("c", "third"));

			HelpFormatter helpFormatter = new HelpFormatter();
			helpFormatter.OptionComparer = new SorterComparer();

			StringWriter output = new StringWriter();
			helpFormatter.Options = opts;
			helpFormatter.Width = 80;
			helpFormatter.CommandLineSyntax = "app";
			helpFormatter.PrintUsage(output);

			Assert.AreEqual("usage: app [-c] [-b] [-a]" + EOL, output.ToString());
		}

		private class SorterComparer : IComparer {
			public int Compare(Object o1, Object o2) {
				// reverses the fuctionality of the default comparator
				Option opt1 = (Option) o1;
				Option opt2 = (Option) o2;
				return String.Compare(opt2.Key, opt1.Key, true);
			}
		}

		[Test]
		public void PrintSortedUsageWithNullComparer() {
			Options opts = new Options();
			opts.AddOption(new Option("a", "first"));
			opts.AddOption(new Option("b", "second"));
			opts.AddOption(new Option("c", "third"));

			HelpFormatter helpFormatter = new HelpFormatter();
			helpFormatter.OptionComparer = null;

			StringWriter output = new StringWriter();
			helpFormatter.Options = opts;
			helpFormatter.CommandLineSyntax = "app";
			helpFormatter.Width = 80;
			helpFormatter.PrintUsage(output);

			Assert.AreEqual("usage: app [-a] [-b] [-c]" + EOL, output.ToString());
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
			formatter.Options = options;
			formatter.CommandLineSyntax = "app";
			formatter.Width = 80;
			formatter.PrintUsage(output);

			Assert.AreEqual("usage: app [-a | -b | -c]" + EOL, output.ToString());
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
			formatter.Options = options;
			formatter.Width = 80;
			formatter.CommandLineSyntax = "app";
			formatter.PrintUsage(output);

			Assert.AreEqual("usage: app -a | -b | -c" + EOL, output.ToString());
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
			formatter.ArgumentName = null;
			formatter.Options = options;
			formatter.CommandLineSyntax = "app";
			formatter.Width = 80;
			formatter.PrintUsage(output);

			Assert.AreEqual("usage: app -f" + EOL, output.ToString());
		}

		[Test]
		public void Rtrim() {
			HelpFormatter formatter = new HelpFormatter();

			Assert.AreEqual(null, formatter.rtrim(null));
			Assert.AreEqual("", formatter.rtrim(""));
			Assert.AreEqual("  foo", formatter.rtrim("  foo  "));
		}

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

		[Test]
		public void HeaderStartingWithLineSeparator() {
			// related to Bugzilla #21215
			Options options = new Options();
			HelpFormatter formatter = new HelpFormatter();
			String header = EOL + "Header";
			String footer = "Footer";
			StringWriter output = new StringWriter();
			formatter.Options = options;
			formatter.Width = 80;
			formatter.Header = header;
			formatter.Footer = footer;
			formatter.LeftPadding = 2;
			formatter.DescriptionPadding = 2;
			formatter.CommandLineSyntax = "foobar";
			formatter.PrintHelp(output, true);
			Assert.AreEqual(
				"usage: foobar" + EOL +
				"" + EOL +
				"Header" + EOL +
				"" + EOL +
				"Footer" + EOL
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
			formatter.Options = options;
			formatter.Width = 80;
			formatter.LeftPadding = 2;
			formatter.DescriptionPadding = 2;
			formatter.CommandLineSyntax = "foobar";
			formatter.PrintHelp(output, true);
			Assert.AreEqual(
				"usage: foobar [-a] [--bbb] [-c]" + EOL +
				"  -a,--aaa  aaaaaaa" + EOL +
				"     --bbb  bbbbbbb" + EOL +
				"  -c        ccccccc" + EOL
				, output.ToString());
		}


		[Test]
		public void OptionWithoutShortFormat2() {
			// related to Bugzilla #27635 (CLI-26)
			Option help = new Option("h", "help", false, "print this message");
			Option version = new Option("v", "version", false, "print version information");
			Option newRun = new Option("n", "new", false, "Create NLT cache entries only for new items");
			Option trackerRun = new Option("t", "tracker", false, "Create NLT cache entries only for tracker items");

			Option timeLimit = OptionBuilder.New().WithLongName("limit")
				.HasArgument()
				.WithValueSeparator()
				.WithDescription("Set time limit for execution, in mintues")
				.Create("l");

			Option age = OptionBuilder.New().WithLongName("age")
				.HasArgument()
				.WithValueSeparator()
				.WithDescription("Age (in days) of cache item before being recomputed")
				.Create("a");

			Option server = OptionBuilder.New().WithLongName("server")
				.HasArgument()
				.WithValueSeparator()
				.WithDescription("The NLT server address")
				.Create("s");

			Option numResults = OptionBuilder.New().WithLongName("results")
				.HasArgument()
				.WithValueSeparator()
				.WithDescription("Number of results per item")
				.Create("r");

			Option configFile = OptionBuilder.New().WithLongName("config")
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
			formatter.ArgumentName = "arg";
			String EOL = Environment.NewLine;
			StringWriter output = new StringWriter();
			formatter.Options = mOptions;
			formatter.Width = 80;
			formatter.Header = "header";
			formatter.Footer = "footer";
			formatter.DescriptionPadding = 2;
			formatter.LeftPadding = 2;
			formatter.CommandLineSyntax = "commandline";
			formatter.PrintHelp(output, true);
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