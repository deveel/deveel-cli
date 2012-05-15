using System;

namespace Deveel.Configuration {
	public class ParseException : Exception {
		public ParseException(string message)
			: base(message) {
		}
	}
}