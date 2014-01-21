using System;

namespace Deveel.Configuration {
	public class UnrecognizedOptionException : ParseException {
		public UnrecognizedOptionException(string message)
			: base(message) {
		}

		public UnrecognizedOptionException(string message, string option)
			: this(message) {
			Option = option;
		}

		public string Option { get; private set; }
	}
}