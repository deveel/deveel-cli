using System;
using System.Collections.Generic;

namespace Deveel.Configuration {
	public interface ICommandLine {
		bool HasParsed { get; }

		IEnumerable<string> Arguments { get; }

		IEnumerable<IOption> Options { get; }

		IOptionValue GetOptionValue(string optionName);
	}
}