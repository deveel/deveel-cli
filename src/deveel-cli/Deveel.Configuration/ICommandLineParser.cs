using System;

namespace Deveel.Configuration {
	public interface ICommandLineParser {
		Options Options { get; set; }
		
		
		CommandLine Parse(string[] arguments);
		
		CommandLine Parse(string[] arguments, bool stopAtNonOption);
	}
}