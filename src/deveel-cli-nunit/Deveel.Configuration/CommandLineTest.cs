using System;
using System.Collections;
using System.Collections.Generic;

using NUnit.Framework;

namespace Deveel.Configuration {
	[TestFixture]
	public class CommandLineTest {
		[Test]
		public void GetOptionProperties() {
			string[] args = new String[] { "-Dparam1=value1", "-Dparam2=value2", "-Dparam3", "-Dparam4=value4", "-D", "--property", "foo=bar" };
			
			Options options = new Options();
			options.AddOption(OptionBuilder.New().WithValueSeparator().hasOptionalArgs(2).Create('D'));
			options.AddOption(OptionBuilder.New().WithValueSeparator().HasArguments(2).WithLongName("property").Create());
			
			Parser parser = new GnuParser(options);
			CommandLine cl = parser.Parse(args);
			
			IDictionary<string, string> props = cl.GetOptionProperties("D");
			
			Assert.IsNotNull(props, "null properties");
			Assert.AreEqual(4, props.Count, "number of properties in " + props);
			Assert.AreEqual("value1", props["param1"], "property 1");
			Assert.AreEqual("value2", props["param2"], "property 2");
			Assert.AreEqual("true", props["param3"], "property 3");
			Assert.AreEqual("value4", props["param4"], "property 4");
			
			Assert.AreEqual("bar", cl.GetOptionProperties("property")["foo"], "property with long format");
		}
	}
}