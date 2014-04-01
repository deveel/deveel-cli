using System;

namespace Deveel.Configuration {
	public interface IOption : IEquatable<IOption> {
		string Name { get; set; }

		string LongName { get; set; }

		OptionType Type { get; set; }

		string Description { get; set; }

		bool IsRequired { get; set; }

		int ArgumentCount { get; set; }

		char ValueSeparator { get; set; }

		string ArgumentName { get; set; }
	}
}