using System;
using System.Collections;

namespace Deveel.Configuration {
	public class GnuParser : Parser {

		public GnuParser() {
		}

		protected override String[] Flatten(Options options, string[] arguments, bool stopAtNonOption) {
			ArrayList tokens = new ArrayList();

			bool eatTheRest = false;

			for (int i = 0; i < arguments.Length; i++) {
				String arg = arguments[i];

				if ("--".Equals(arg)) {
					eatTheRest = true;
					tokens.Add("--");
				} else if ("-".Equals(arg)) {
					tokens.Add("-");
				} else if (arg.StartsWith("-")) {
					String opt = Util.StripLeadingHyphens(arg);

					if (options.HasOption(opt)) {
						tokens.Add(arg);
					} else {
						if (opt.IndexOf('=') != -1 && options.HasOption(opt.Substring(0, opt.IndexOf('=')))) {
							// the format is --foo=value or -foo=value
							tokens.Add(arg.Substring(0, arg.IndexOf('='))); // --foo
							tokens.Add(arg.Substring(arg.IndexOf('=') + 1)); // value
						} else if (options.HasOption(arg.Substring(0, 2))) {
							// the format is a special properties option (-Dproperty=value)
							tokens.Add(arg.Substring(0, 2)); // -D
							tokens.Add(arg.Substring(2)); // property=value
						} else {
							eatTheRest = stopAtNonOption;
							tokens.Add(arg);
						}
					}
				} else {
					tokens.Add(arg);
				}

				if (eatTheRest) {
					for (i++; i < arguments.Length; i++) {
						tokens.Add(arguments[i]);
					}
				}
			}

			return (String[]) tokens.ToArray(typeof (string));
		}
	}

}