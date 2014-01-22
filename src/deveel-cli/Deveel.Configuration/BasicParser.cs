using System;

namespace Deveel.Configuration {
	public sealed class BasicParser : Parser {		
		protected override string[] Flatten(Options options, string[] arguments, bool stopAtNonOption) {
			return arguments;
		}
	}
}