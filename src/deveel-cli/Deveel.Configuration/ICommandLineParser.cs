using System;
using System.Collections.Generic;

namespace Deveel.Configuration {
	public interface ICommandLineParser {		
		ICommandLine Parse(Options options, string[] arguments, IEnumerable<KeyValuePair<string, string>> properties, bool stopAtNonOption);
	}
}