using System;
using System.Collections;
using System.Text;

namespace Deveel.Configuration {
	public class MissingOptionException : ParseException {
		private readonly IList missingOptions;

		public MissingOptionException(String message)
			: base(message) {
		}

		public MissingOptionException(IList missingOptions)
			: this(CreateMessage(missingOptions)) {
			this.missingOptions = missingOptions;
		}

		public IList MissingOptions {
			get { return ArrayList.ReadOnly(missingOptions); }
		}

		private static String CreateMessage(IList missingOptions) {
			StringBuilder buff = new StringBuilder("Missing required option");
			buff.Append(missingOptions.Count == 1 ? "" : "s");
			buff.Append(": ");

			for (int i = 0; i < missingOptions.Count; i++) {
				buff.Append(missingOptions[i]);
				if (i < missingOptions.Count - 1) {
					buff.Append(", ");
				}
			}

			return buff.ToString();
		}
	}
}