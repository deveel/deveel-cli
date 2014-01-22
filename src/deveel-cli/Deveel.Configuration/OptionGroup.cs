using System;
using System.Collections.Generic;

namespace Deveel.Configuration {
	[Serializable]
	public class OptionGroup : IOptionGroup {
		private readonly IDictionary<string, IOption> optionMap = new Dictionary<string, IOption>();

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

		public bool IsRequired { get; set; }
	}
}