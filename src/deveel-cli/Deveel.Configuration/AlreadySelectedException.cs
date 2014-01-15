using System;

namespace Deveel.Configuration {
	public class AlreadySelectedException : ParseException {
		private readonly OptionGroup group;

		public AlreadySelectedException(String message)
			: base(message) {
		}

		public AlreadySelectedException(OptionGroup group, IOption option)
			: this("The option '" + option.Key() + "' was specified but an option from this group "
				   + "has already been selected: '" + group.Selected + "'") {
			this.group = group;
			this.Option = option;
		}

		public OptionGroup OptionGroup {
			get { return group; }
		}

		public IOption Option { get; private set; }
	}
}