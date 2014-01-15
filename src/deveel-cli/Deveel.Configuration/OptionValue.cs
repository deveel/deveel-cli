using System;
using System.Collections.Generic;

namespace Deveel.Configuration {
	public class OptionValue : IOptionValue {
		private List<string> values = new List<string>();

		internal OptionValue(IOption option) {
			Option = option;
		}

		public IOption Option { get; private set; }

		internal void AddValueForProcessing(String value) {
			if (Option.ArgumentCount == Configuration.Option.Unitialized)
				throw new ApplicationException("NO_ARGS_ALLOWED");

			ProcessValue(value);
		}

		private void ProcessValue(String value) {
			// this Option has a separator character
			if (Option.HasValueSeparator()) {
				// get the separator character
				char sep = Option.ValueSeparator;

				// store the index for the value separator
				int index = value.IndexOf(sep);

				// while there are more value separators
				while (index != -1) {
					// next value to be added 
					if (values.Count == (Option.ArgumentCount - 1)) {
						break;
					}

					// store
					Add(value.Substring(0, index));

					// parse
					value = value.Substring(index + 1);

					// get new index
					index = value.IndexOf(sep);
				}
			}

			// store the actual value or the last value that has been parsed
			Add(value);
		}

		private void Add(String value) {
			if ((Option.ArgumentCount > 0) && (values.Count > (Option.ArgumentCount - 1))) {
				throw new ApplicationException("Cannot add value, list full.");
			}

			// store value
			values.Add(value);
		}


		public string Value {
			get { return HasNoValues ? null : values[0]; }
		}

		public IEnumerable<string> Values {
			get { return HasNoValues ? null : values.ToArray(); }
		}

		private bool HasNoValues {
			get { return values.Count == 0; }
		}

		public object Clone() {
			var obj = (OptionValue) MemberwiseClone();
			obj.values = new List<string>(values);
			return obj;
		}

		internal void ClearValues() {
			values.Clear();
		}
	}
}