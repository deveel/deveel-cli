using System;

namespace Deveel.Configuration {
	public interface ICommandLineParser {
		Options Options { get; set; }
				
		ICommandLine Parse(string[] arguments, bool stopAtNonOption);
	}
}