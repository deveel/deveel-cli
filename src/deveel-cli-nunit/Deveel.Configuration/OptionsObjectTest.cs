using System;

using NUnit.Framework;

namespace Deveel.Configuration {
	[TestFixture]
	public class OptionsObjectTest {
		[Test]
		public void SimpleOptionsTest() {
			var parser = new GnuParser();

			var args = new string[] {"--foo", "bar", "--Value"};
			var parsed = parser.ParseObject<SimpleOptions>(args);

			Assert.IsAssignableFrom<SimpleOptions>(parsed);
			Assert.AreEqual("bar", parsed.foo);
			Assert.AreEqual(null, parsed.Value);
		}

		[Test]
		public void ConfiguredOptionsTest() {

		}

		#region SimpleOptions

		class SimpleOptions {
			public string foo;

			public bool Value { get; set; }
		}

		#endregion
	}
}
