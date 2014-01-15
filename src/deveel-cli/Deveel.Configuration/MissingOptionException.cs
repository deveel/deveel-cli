using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Deveel.Configuration {
	public class MissingOptionException : ParseException {
		private readonly IList<string> missingOptions;

		public MissingOptionException(String message)
			: base(message) {
		}

		public MissingOptionException(IList<string> missingOptions)
			: this(CreateMessage(missingOptions)) {
			this.missingOptions = missingOptions;
		}

		public IList<string> MissingOptions {
			get { return missingOptions.ToList().AsReadOnly(); }
		}

		private static String CreateMessage(IList<string> missingOptions) {
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