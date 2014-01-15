using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

namespace Deveel.Configuration {
	[TestFixture(ParserStyle.Basic)]
	[TestFixture(ParserStyle.Posix)]
	[TestFixture(ParserStyle.Gnu)]
	public class ParserTestCase {
		public ParserTestCase(ParserStyle style) {
			this.style = style;
		}
		
		private readonly ParserStyle style;
		protected Parser parser;
		protected Options options;

		[SetUp]
		public virtual void SetUp() {
			options = new Options()
				.AddOption("a", "enable-a", false, "turn [a] on or off")
				.AddOption("b", "bfile", true, "set the value of [b]")
				.AddOption("c", "copt", false, "turn [c] on or off");
			
			if (style == ParserStyle.Basic)
				parser = new BasicParser(options);
			else if (style == ParserStyle.Posix)
				parser = new PosixParser(options);
			else if (style == ParserStyle.Gnu)
				parser = new GnuParser(options);
		}

		[Test]
		public void SimpleShort() {
			String[] args = new String[] { "-a",
                                       "-b", "toast",
                                       "foo", "bar" };

			CommandLine cl = parser.Parse(args);

			Assert.IsTrue(cl.HasOption("a"), "Confirm -a is set");
			Assert.IsTrue(cl.HasOption("b"), "Confirm -b is set");
			Assert.IsTrue(cl.GetOptionValue("b").Value.Equals("toast"), "Confirm arg of -b");
			Assert.IsTrue(cl.Arguments.Count() == 2, "Confirm size of extra args");
		}

		[Test]
		public void SimpleLong() {
			String[] args = new String[] { "--enable-a",
                                       "--bfile", "toast",
                                       "foo", "bar" };

			CommandLine cl = parser.Parse(args);

			Assert.IsTrue(cl.HasOption("a"), "Confirm -a is set");
			Assert.IsTrue(cl.HasOption("b"), "Confirm -b is set");
			Assert.IsTrue(cl.GetOptionValue("b").Value.Equals("toast"), "Confirm arg of -b");
			Assert.IsTrue(cl.GetOptionValue("bfile").Value.Equals("toast"), "Confirm arg of --bfile");
			Assert.IsTrue(cl.Arguments.Count() == 2, "Confirm size of extra args");
		}

		[Test]
		public void Multiple() {
			String[] args = new String[] { "-c",
                                       "foobar",
                                       "-b", "toast" };

			CommandLine cl = parser.Parse(args, true);
			Assert.IsTrue(cl.HasOption("c"), "Confirm -c is set");
			Assert.IsTrue(cl.Arguments.Count() == 3, "Confirm  3 extra args: " + cl.Arguments.Count());

			cl = parser.Parse( cl.Arguments.ToArray());

			Assert.IsTrue(!cl.HasOption("c"), "Confirm -c is not set");
			Assert.IsTrue(cl.HasOption("b"), "Confirm -b is set");
			Assert.IsTrue(cl.GetOptionValue("b").Value.Equals("toast"), "Confirm arg of -b");
			Assert.IsTrue(cl.Arguments.Count() == 1, "Confirm  1 extra arg: " + cl.Arguments.Count());
			Assert.IsTrue(cl.Arguments.First().Equals("foobar"), "Confirm  value of extra arg: " + cl.Arguments.First());
		}

		[Test]
		public void MultipleWithLong() {
			String[] args = new String[] { "--copt",
                                       "foobar",
                                       "--bfile", "toast" };

			CommandLine cl = parser.Parse(args, true);
			Assert.IsTrue(cl.HasOption("c"), "Confirm -c is set");
			Assert.IsTrue(cl.Arguments.Count() == 3, "Confirm  3 extra args: " + cl.Arguments.Count());

			parser.Options = options;
			cl = parser.Parse(cl.Arguments.ToArray());

			Assert.IsTrue(!cl.HasOption("c"), "Confirm -c is not set");
			Assert.IsTrue(cl.HasOption("b"), "Confirm -b is set");
			Assert.IsTrue(cl.GetOptionValue("b").Value.Equals("toast"), "Confirm arg of -b");
			Assert.IsTrue(cl.Arguments.Count() == 1, "Confirm  1 extra arg: " + cl.Arguments.Count());
			Assert.IsTrue(cl.Arguments.First().Equals("foobar"), "Confirm  value of extra arg: " + cl.Arguments.First());
		}

		[Test]
		public void UnrecognizedOption() {
			String[] args = new String[] { "-a", "-d", "-b", "toast", "foo", "bar" };

			try {
				parser.Parse(args);
				Assert.Fail("UnrecognizedOptionException wasn't thrown");
			} catch (UnrecognizedOptionException e) {
				Assert.AreEqual("-d", e.Option);
			}
		}

		[Test]
		public void MissingArg() {
			String[] args = new String[] { "-b" };

			bool caught = false;

			try {
				parser.Parse(args);
			} catch (MissingArgumentException e) {
				caught = true;
				Assert.AreEqual("b", e.Option.Name, "option missing an argument");
			}

			Assert.IsTrue(caught, "Confirm MissingArgumentException caught");
		}

		[Test]
		public void DoubleDash() {
			String[] args = new String[] { "--copt",
                                       "--",
                                       "-b", "toast" };

			CommandLine cl = parser.Parse(args);

			Assert.IsTrue(cl.HasOption("c"), "Confirm -c is set");
			Assert.IsTrue(!cl.HasOption("b"), "Confirm -b is not set");
			Assert.IsTrue(cl.Arguments.Count() == 2, "Confirm 2 extra args: " + cl.Arguments.Count());
		}

		[Test]
		public void SingleDash() {
			String[] args = new String[] { "--copt",
                                       "-b", "-",
                                       "-a",
                                       "-" };

			CommandLine cl = parser.Parse( args);

			Assert.IsTrue(cl.HasOption("a"), "Confirm -a is set");
			Assert.IsTrue(cl.HasOption("b"), "Confirm -b is set");
			Assert.IsTrue(cl.GetOptionValue("b").Value.Equals("-"), "Confirm arg of -b");
			Assert.IsTrue(cl.Arguments.Count() == 1, "Confirm 1 extra arg: " + cl.Arguments.Count());
			Assert.IsTrue(cl.Arguments.First().Equals("-"), "Confirm value of extra arg: " + cl.Arguments.First());
		}

		[Test]
		public void StopAtUnexpectedArg() {
			String[] args = new String[] { "-c",
                                       "foober",
                                       "-b",
                                       "toast" };

			CommandLine cl = parser.Parse(args, true);
			Assert.IsTrue(cl.HasOption("c"), "Confirm -c is set");
			Assert.IsTrue(cl.Arguments.Count() == 3, "Confirm  3 extra args: " + cl.Arguments.Count());
		}

		[Test]
		public void StopAtExpectedArg() {
			String[] args = new String[] { "-b", "foo" };

			CommandLine cl = parser.Parse(args, true);

			Assert.IsTrue(cl.HasOption('b'), "Confirm -b is set");
			Assert.AreEqual("foo", cl.GetOptionValue('b').Value, "Confirm -b is set");
			Assert.IsTrue(!cl.Arguments.Any(), "Confirm no extra args: " + cl.Arguments.Count());
		}

		[Test]
		public void StopAtNonOptionShort() {
			String[] args = new String[]{"-z",
                                     "-a",
                                     "-btoast"};

			CommandLine cl = parser.Parse(args, true);
			Assert.IsFalse(cl.HasOption("a"), "Confirm -a is not set");
			Assert.IsTrue(cl.Arguments.Count() == 3, "Confirm  3 extra args: " + cl.Arguments.Count());
		}

		[Test]
		public void StopAtNonOptionLong() {
			String[] args = new String[]{"--zop==1",
                                     "-abtoast",
                                     "--b=bar"};
			
			CommandLine cl = parser.Parse(args, true);

			Assert.IsFalse(cl.HasOption("a"), "Confirm -a is not set");
			Assert.IsFalse(cl.HasOption("b"), "Confirm -b is not set");
			Assert.IsTrue(cl.Arguments.Count() == 3, "Confirm  3 extra args: " + cl.Arguments.Count());
		}

		[Test]
		public void NegativeArgument() {
			String[] args = new String[] { "-b", "-1" };

			CommandLine cl = parser.Parse(args);
			Assert.AreEqual("-1", cl.GetOptionValue("b").Value);
		}

		[Test]
		public void ArgumentStartingWithHyphen() {
			String[] args = new String[] { "-b", "-foo" };

			CommandLine cl = parser.Parse(args);
			Assert.AreEqual("-foo", cl.GetOptionValue("b").Value);
		}

		[Test]
		public void ShortWithEqual() {
			if (style == ParserStyle.Basic)
				return;
			
			String[] args = new String[] { "-f=bar" };

			Options options = new Options();
			options.AddOption(OptionBuilder.New().WithLongName("foo").HasArgument().Create('f'));

			parser.Options = options;
			CommandLine cl = parser.Parse(args);

			Assert.AreEqual("bar", cl.GetOptionValue("foo").Value);
		}

		[Test]
		public void ShortWithoutEqual() {
			if (style == ParserStyle.Basic)
				return;
			
			String[] args = new String[] { "-fbar" };

			Options options = new Options();
			options.AddOption(OptionBuilder.New().WithLongName("foo").HasArgument().Create('f'));

			parser.Options = options;
			CommandLine cl = parser.Parse(args);

			Assert.AreEqual("bar", cl.GetOptionValue("foo").Value);
		}

		[Test]
		public void LongWithEqual() {
			if (style == ParserStyle.Basic)
				return;
			
			String[] args = new String[] { "--foo=bar" };

			Options options = new Options();
			options.AddOption(OptionBuilder.New().WithLongName("foo").HasArgument().Create('f'));

			parser.Options = options;
			CommandLine cl = parser.Parse(args);

			Assert.AreEqual("bar", cl.GetOptionValue("foo").Value);
		}

		[Test]
		public void LongWithEqualSingleDash() {
			if (style == ParserStyle.Basic)
				return;
			
			String[] args = new String[] { "-foo=bar" };

			Options options = new Options();
			options.AddOption(OptionBuilder.New().WithLongName("foo").HasArgument().Create('f'));

			parser.Options = options;
			CommandLine cl = parser.Parse(args);

			Assert.AreEqual("bar", cl.GetOptionValue("foo").Value);
		}

		[Test]
		public void PropertiesOption() {
			if (style == ParserStyle.Basic)
				return;
			
			String[] args = new String[] { "-Jsource=1.5", "-J", "target", "1.5", "foo" };

			Options options = new Options();
			options.AddOption(OptionBuilder.New().WithValueSeparator().HasArguments(2).Create('J'));

			parser.Options = options;
			CommandLine cl = parser.Parse(args);

			IList values = cl.GetOptionValues("J");
			Assert.IsNotNull(values, "null values");
			Assert.AreEqual(4, values.Count, "number of values");
			Assert.AreEqual("source", values[0], "value 1");
			Assert.AreEqual("1.5", values[1], "value 2");
			Assert.AreEqual("target", values[2], "value 3");
			Assert.AreEqual("1.5", values[3], "value 4");
			IEnumerable<string> argsleft = cl.Arguments;
			Assert.AreEqual(1, argsleft.Count(), "Should be 1 arg left");
			Assert.AreEqual("foo", argsleft.First(), "Expecting foo");
		}
	}
}