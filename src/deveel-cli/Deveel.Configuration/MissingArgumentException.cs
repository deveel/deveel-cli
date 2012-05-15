using System;

namespace Deveel.Configuration {
	public class MissingArgumentException : ParseException {
		private readonly Option option;

		public MissingArgumentException(String message)
			: base(message) {
		}

		public MissingArgumentException(Option option)
			: this("Missing argument for option: " + option.Key) {
			this.option = option;
		}

		public Option Option {
			get { return option; }
		}
	}
}