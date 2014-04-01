using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Deveel.Configuration {
	public class MissingOptionException : ParseException {
		private readonly IEnumerable<string> missingOptions;

		public MissingOptionException(String message)
			: base(message) {
		}

		public MissingOptionException(IEnumerable<string> missingOptions)
			: this(CreateMessage(missingOptions)) {
			this.missingOptions = missingOptions;
		}

		public IList<string> MissingOptions {
			get { return missingOptions.ToList().AsReadOnly(); }
		}
		private static String CreateMessage(IEnumerable<string> missingOptions) {
			var buff = new StringBuilder("Missing required option ");
		    var list = new List<string>(missingOptions);

			buff.Append(list.Count == 1 ? "" : "s");
			buff.Append(": ");

			for (int i = 0; i < list.Count; i++) {
				buff.Append(list[i]);
				if (i < list.Count - 1) {
					buff.Append(", ");
				}
			}

			return buff.ToString();
		}
	}
}