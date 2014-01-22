using System;
using System.Linq;

using NUnit.Framework;

namespace Deveel.Configuration {
	[TestFixture]
	public class ArgumentIsOptionTest {
		private Options options;
		private ICommandLineParser parser;

		[SetUp]
		public void SetUp() {
			options = new Options().AddOption("p", false, "Option p").AddOption("attr", true, "Option accepts argument");

			parser = new PosixParser();
		}

		[Test]
		public void OptionAndOptionWithArgument() {
			String[] args = new String[] {
			                             	"-p",
			                             	"-attr",
			                             	"p"
			                             };

			ICommandLine cl = parser.Parse(options, args);
			Assert.IsTrue(cl.HasOption("p"), "Confirm -p is set");
			Assert.IsTrue(cl.HasOption("attr"), "Confirm -attr is set");
			Assert.IsTrue(cl.GetOptionValue("attr").Value.Equals("p"), "Confirm arg of -attr");
			Assert.IsTrue(!cl.Arguments.Any(), "Confirm all arguments recognized");
		}

		[Test]
		public void OptionWithArgument() {
			String[] args = new String[]{ "-attr", "p" };

			ICommandLine cl = parser.Parse(options, args);
			Assert.IsFalse(cl.HasOption("p"), "Confirm -p is set");
			Assert.IsTrue(cl.HasOption("attr"), "Confirm -attr is set");
			Assert.IsTrue(cl.GetOptionValue("attr").Value.Equals("p"), "Confirm arg of -attr");
			Assert.IsTrue(!cl.Arguments.Any(), "Confirm all arguments recognized");
		}

		[Test]
		public void Option() {
			String[] args = new String[]{ "-p" };

			ICommandLine cl = parser.Parse(options, args);
			Assert.IsTrue(cl.HasOption("p"), "Confirm -p is set");
			Assert.IsFalse(cl.HasOption("attr"), "Confirm -attr is not set");
			Assert.IsTrue(!cl.Arguments.Any(), "Confirm all arguments recognized");
		}
	}
}