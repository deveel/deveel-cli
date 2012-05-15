using System;

namespace Deveel.Configuration {
	public class UnrecognizedOptionException : ParseException {
		private readonly String option;

		public UnrecognizedOptionException(String message)
			: base(message) {
		}

		public UnrecognizedOptionException(String message, String option)
			: this(message) {
			this.option = option;
		}

		public string Option {
			get { return option; }
		}
	}
}