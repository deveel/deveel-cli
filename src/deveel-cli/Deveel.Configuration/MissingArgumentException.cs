using System;

namespace Deveel.Configuration {
	public class MissingArgumentException : ParseException {
		public MissingArgumentException(String message)
			: base(message) {
		}

		public MissingArgumentException(IOption option)
			: this("Missing argument for option: " + option.Key()) {
			this.Option = option;
		}

		public IOption Option { get; private set; }
	}
}