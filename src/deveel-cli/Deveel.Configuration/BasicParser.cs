using System;

namespace Deveel.Configuration {
	public sealed class BasicParser : Parser {
		public BasicParser(Options options)
			: base(options) {
		}

		public BasicParser() {
		}
		
		protected override string[] Flatten(string[] arguments, bool stopAtNonOption) {
			return arguments;
		}
	}
}