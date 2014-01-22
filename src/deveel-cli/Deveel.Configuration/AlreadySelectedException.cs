using System;

namespace Deveel.Configuration {
	public class AlreadySelectedException : ParseException {
		public AlreadySelectedException(String message)
			: base(message) {
		}

		public AlreadySelectedException(IOptionGroup group, IOption option, IOption selected)
			: this("The option '" + option.Key() + "' was specified but an option from this group "
				   + "has already been selected: '" + selected.Key() + "'") {
			OptionGroup = group;
			Option = option;
			SelectedOption = selected;
		}

		public IOptionGroup OptionGroup { get; private set; }

		public IOption Option { get; private set; }

		public IOption SelectedOption { get; private set; }
	}
}