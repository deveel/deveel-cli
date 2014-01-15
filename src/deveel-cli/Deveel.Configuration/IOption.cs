using System;

namespace Deveel.Configuration {
	public interface IOption : IEquatable<IOption> {
		string Name { get; }

		string LongName { get; }

		OptionType Type { get; }

		string Description { get;  }

		bool IsRequired { get; }

		int ArgumentCount { get; }

		char ValueSeparator { get; }
	}
}