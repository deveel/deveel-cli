using System;
using System.Collections.Generic;

namespace Deveel.Configuration {
	[Serializable]
	public class OptionGroup : IOptionGroup {
		private readonly IDictionary<string, IOption> optionMap = new Dictionary<string, IOption>();
		private String selected;

		public OptionGroup AddOption(IOption option) {
			// key   - option name
			// value - the option
			optionMap[option.Key()] = option;

			return this;
		}


		public IEnumerable<IOption> Options {
			get {
				// the values are the collection of options
				return optionMap.Values;
			}
		}

		internal void SetSelected(IOption option) {
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

		public bool IsRequired { get; set; }
	}
}