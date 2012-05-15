using System;
using System.Collections;

namespace Deveel.Configuration {
	[Serializable]
	public class OptionGroup {
		private readonly IDictionary optionMap = new Hashtable();
		private String selected;
		private bool required;

		public OptionGroup AddOption(Option option) {
			// key   - option name
			// value - the option
			optionMap[option.Key] = option;

			return this;
		}

		public ICollection Names {
			get {
				// the key set is the collection of names
				return optionMap.Keys;
			}
		}

		public ICollection Options {
			get {
				// the values are the collection of options
				return optionMap.Values;
			}
		}

		public void SetSelected(Option option) {
			// if no option has already been selected or the 
			// same option is being reselected then set the
			// selected member variable
			if (selected == null || selected.Equals(option.Name)) {
				selected = option.Name;
			} else {
				throw new AlreadySelectedException(this, option);
			}
		}

		public string Selected {
			get { return selected; }
		}

		public bool IsRequired {
			set { required = value; }
			get { return required; }
		}
	}
}