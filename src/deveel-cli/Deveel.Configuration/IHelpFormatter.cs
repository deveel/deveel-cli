using System;
using System.IO;

namespace Deveel.Configuration {
	public interface IHelpFormatter {
		void PrintHelp(Options options, HelpSettings settings, TextWriter writer, bool autoUsage);
	}
}