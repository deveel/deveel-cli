using System;

namespace Deveel.Configuration {
	public class AlreadySelectedException : ParseException {
		private readonly OptionGroup group;
		private readonly Option option;

		public AlreadySelectedException(String message)
			: base(message) {
		}

		public AlreadySelectedException(OptionGroup group, Option option)
			: this("The option '" + option.Key + "' was specified but an option from this group "
				   + "has already been selected: '" + group.Selected + "'") {
			this.group = group;
			this.option = option;
		}

		public OptionGroup OptionGroup {
			get { return group; }
		}

		public Option Option {
			get { return option; }
		}
	}
}