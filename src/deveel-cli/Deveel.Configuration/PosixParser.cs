using System;
using System.Collections;

namespace Deveel.Configuration {
	public class PosixParser : Parser {
		private ArrayList tokens = new ArrayList();
		private bool eatTheRest;
		private Option currentOption;
		
		public PosixParser(Options options)
			: base(options) {
		}
		
		public PosixParser() {
		}

		private void Init() {
			eatTheRest = false;
			tokens.Clear();
		}

		protected override String[] Flatten(String[] arguments, bool stopAtNonOption) {
			Init();

			int argc = arguments.Length;
			
			for (int i = 0; i < argc; i++) {
				// get the next command line token
				string token = arguments[i];

				// handle long option --foo or --foo=bar
				if (token.StartsWith("--")) {
					int pos = token.IndexOf('=');
					String opt = pos == -1 ? token : token.Substring(0, pos); // --foo

					if (!Options.HasOption(opt)) {
						ProcessNonOptionToken(token, stopAtNonOption);
					} else {
						currentOption = Options.GetOption(opt);

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
					if (token.Length == 2 || Options.HasOption(token)) {
						ProcessOptionToken(token, stopAtNonOption);
					}
						// requires bursting
					else {
						BurstToken(token, stopAtNonOption);
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
			if (stopAtNonOption && (currentOption == null || !currentOption.HasArgument)) {
				eatTheRest = true;
				tokens.Add("--");
			}

			tokens.Add(value);
		}

		private void ProcessOptionToken(String token, bool stopAtNonOption) {
			if (stopAtNonOption && !Options.HasOption(token)) {
				eatTheRest = true;
			}

			if (Options.HasOption(token)) {
				currentOption = Options.GetOption(token);
			}

			tokens.Add(token);
		}

		protected void BurstToken(String token, bool stopAtNonOption) {
			for (int i = 1; i < token.Length; i++) {
				String ch = token[i].ToString();

				if (Options.HasOption(ch)) {
					tokens.Add("-" + ch);
					currentOption = Options.GetOption(ch);

					if (currentOption.HasArgument && (token.Length != (i + 1))) {
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