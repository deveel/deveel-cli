using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Deveel.Configuration {
	public abstract class Parser : ICommandLineParser {
		private CommandLine cmd;
		private Options options;
		private IList<string> requiredOptions;
		private IDictionary<string, OptionValue> values = new Dictionary<string, OptionValue>();

		
		protected Parser(Options options) {
			Options = options;
		}
		
		protected Parser() {
		}

		public Options Options {
			get { return options; }
			set {
				options = value;
				if (value != null)
					requiredOptions = new List<string>(value.RequiredOptions);
			}
		}

		protected IList<string> RequiredOptions {
			get { return requiredOptions; }
		}

		protected abstract String[] Flatten(string[] arguments, bool stopAtNonOption);

		public CommandLine Parse(string[] arguments) {
			return Parse(arguments, null, false);
		}

		public CommandLine Parse(string[] arguments, IDictionary properties) {
			return Parse(arguments, properties, false);
		}

		public ICommandLine Parse(string[] arguments, bool stopAtNonOption) {
			return Parse(arguments, null, stopAtNonOption);
		}

		public CommandLine Parse(string[] arguments, IDictionary properties, bool stopAtNonOption) {
			if (options == null)
				return new CommandLine(false);

			if (values != null) {
				// clear out the data in options in case it's been used before
				foreach (OptionValue option in values.Values) {
					option.ClearValues();
				}
			}

			cmd = new CommandLine(true);

			bool eatTheRest = false;

			if (arguments == null)
				arguments = new string[0];

			string[] tokenList = Flatten(arguments, stopAtNonOption);

			int tokenCount = tokenList.Length;

			for (int i = 0; i < tokenCount; i++) {
				string t = tokenList[i];

				// the value is the double-dash
				if ("--".Equals(t)) {
					eatTheRest = true;
				}

				// the value is a single dash
				else if ("-".Equals(t)) {
					if (stopAtNonOption) {
						eatTheRest = true;
					} else {
						cmd.AddArgument(t);
					}
				}

				// the value is an option
				else if (t.StartsWith("-")) {
					if (stopAtNonOption && !Options.HasOption(t)) {
						eatTheRest = true;
						cmd.AddArgument(t);
					} else {
						ProcessOption(t, tokenList, ref i);
					}
				}

				// the value is an argument
				else {
					cmd.AddArgument(t);

					if (stopAtNonOption) {
						eatTheRest = true;
					}
				}

				// eat the remaining tokens
				if (eatTheRest) {
					while (++i < tokenCount) {
						String str = tokenList[i];

						// ensure only one double-dash is added
						if (!"--".Equals(str)) {
							cmd.AddArgument(str);
						}
					}
				}
			}

			ProcessProperties(properties);
			CheckRequiredOptions();

			return cmd;
		}

		private OptionValue SafeGetOptionValue(IOption option) {
			if (values == null)
				values = new Dictionary<string, OptionValue>();

			OptionValue value;
			if (!values.TryGetValue(option.Key(), out value)) {
				values[option.Key()] = value = new OptionValue(option);
			}

			return value;
		}

		protected void ProcessProperties(IDictionary properties) {
			if (properties == null) {
				return;
			}

			foreach (string option in properties.Keys) {
				if (!cmd.HasOption(option)) {
					Option opt = Options.GetOption(option);

					// get the value from the properties instance
					var value = (string)properties[option];
					OptionValue optValue = null;
					if (opt.HasArgument()) {
						optValue = SafeGetOptionValue(opt);

						if (optValue.Values == null || !optValue.Values.Any()) {
							try {
								optValue.AddValueForProcessing(value);
							} catch (ApplicationException) {
								// if we cannot add the value don't worry about it
							}
						}
					} else if (String.Compare(value, "yes", StringComparison.OrdinalIgnoreCase) != 0 &&
							 String.Compare(value, "true", StringComparison.OrdinalIgnoreCase) != 0 &&
							 String.Compare(value, "1", StringComparison.OrdinalIgnoreCase) != 0) {
						// if the value is not yes, true or 1 then don't add the
						// option to the CommandLine
						break;
					}

					cmd.AddOption(optValue);
				}
			}
		}

		protected void CheckRequiredOptions() {
			// if there are required options that have not been processsed
			if (RequiredOptions.Count != 0) {
				throw new MissingOptionException(RequiredOptions);
			}
		}

		public void ProcessArguments(OptionValue opt, string[] tokens, ref int index) {
			// loop until an option is found
			while (++index < tokens.Length) {
				String str = tokens[index];

				// found an Option, not an argument
				if (Options.HasOption(str) && str.StartsWith("-")) {
					index--;
					break;
				}

				// found a value
				try {
					opt.AddValueForProcessing(Util.StripLeadingAndTrailingQuotes(str));
				} catch (ApplicationException) {
					index--;
					break;
				}
			}

			if (opt.Values == null && !opt.Option.HasOptionalArguments()) {
				throw new MissingArgumentException(opt.Option);
			}
		}

		protected void ProcessOption(string arg, string[] tokens, ref int index) {
			bool hasOption = Options.HasOption(arg);

			// if there is no option throw an UnrecognisedOptionException
			if (!hasOption) {
				throw new UnrecognizedOptionException("Unrecognized option: " + arg, arg);
			}

			// get the option represented by arg
			OptionValue opt = new OptionValue(Options.GetOption(arg));

			// if the option is a required option remove the option from
			// the requiredOptions list
			if (opt.Option.IsRequired) {
				RequiredOptions.Remove(opt.Option.Key());
			}

			// if the option is in an OptionGroup make that option the selected
			// option of the group
			if (Options.GetOptionGroup(opt.Option) != null) {
				OptionGroup group = Options.GetOptionGroup(opt.Option);

				if (group.IsRequired) {
					foreach (var option in group.Options)
						RequiredOptions.Remove(option.Key());
				}

				group.SetSelected(opt.Option);
			}

			// if the option takes an argument value
			if (opt.Option.HasArgument()) {
				ProcessArguments(opt, tokens, ref index);
			}

			// set the option on the command line
			cmd.AddOption(opt);
		}

		public static Parser Create(ParseStyle parseStyle) {
			if (parseStyle == ParseStyle.Basic)
				return new BasicParser();
			if (parseStyle == ParseStyle.Gnu)
				return new GnuParser();
			if (parseStyle == ParseStyle.Posix)
				return new PosixParser();

			throw new ArgumentException("The parse style is not supported");
		}
	}
}