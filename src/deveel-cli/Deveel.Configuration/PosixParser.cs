using System;
using System.Collections;

namespace Deveel.Configuration {
	public class PosixParser : Parser {
		private ArrayList tokens = new ArrayList();
		private bool eatTheRest;
		private IOption currentOption;
		
		private void Init() {
			eatTheRest = false;
			tokens.Clear();
		}

		protected override String[] Flatten(Options options, string[] arguments, bool stopAtNonOption) {
			Init();

			int argc = arguments.Length;
			
			for (int i = 0; i < argc; i++) {
				// get the next command line token
				string token = arguments[i];

				// handle long option --foo or --foo=bar
				if (token.StartsWith("--")) {
					int pos = token.IndexOf('=');
					String opt = pos == -1 ? token : token.Substring(0, pos); // --foo

					if (!options.HasOption(opt)) {
						ProcessNonOptionToken(token, stopAtNonOption);
					} else {
						currentOption = options.GetOption(opt);

						tokens.Add(opt);
						if (pos != -1) {
							tokens.Add(token.Substring(pos + 1));
						}
					}
				}

				// single hyphen
				else if ("-".Equals(token)) {
					tokens.Add(token);
				} else if (token.StartsWith("-")) {
					if (token.Length == 2 || options.HasOption(token)) {
						ProcessOptionToken(options, token, stopAtNonOption);
					}
						// requires bursting
					else {
						BurstToken(options, token, stopAtNonOption);
					}
				} else {
					ProcessNonOptionToken(token, stopAtNonOption);
				}

				Gobble(arguments, ref i);
			}

			return (String[])tokens.ToArray(typeof(String));
		}

		private void Gobble(string[] args, ref int index) {
			if (eatTheRest) {
				int argc = args.Length;
				while(++index < argc) {
					tokens.Add(args[index]);
				}
			}
		}

		private void ProcessNonOptionToken(String value, bool stopAtNonOption) {
			if (stopAtNonOption && (currentOption == null || !currentOption.HasArgument())) {
				eatTheRest = true;
				tokens.Add("--");
			}

			tokens.Add(value);
		}

		private void ProcessOptionToken(Options options, string token, bool stopAtNonOption) {
			if (stopAtNonOption && !options.HasOption(token)) {
				eatTheRest = true;
			}

			if (options.HasOption(token)) {
				currentOption = options.GetOption(token);
			}

			tokens.Add(token);
		}

		protected void BurstToken(Options options, string token, bool stopAtNonOption) {
			for (int i = 1; i < token.Length; i++) {
				String ch = token[i].ToString();

				if (options.HasOption(ch)) {
					tokens.Add("-" + ch);
					currentOption = options.GetOption(ch);

					if (currentOption.HasArgument() && (token.Length != (i + 1))) {
						tokens.Add(token.Substring(i + 1));

						break;
					}
				} else if (stopAtNonOption) {
					ProcessNonOptionToken(token.Substring(i), true);
					break;
				} else {
					tokens.Add(token);
					break;
				}
			}
		}
	}
}