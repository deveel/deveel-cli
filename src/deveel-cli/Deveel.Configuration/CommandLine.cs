using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Deveel.Configuration {
	[Serializable]
	public class CommandLine : ICommandLine, IEnumerable {
		private readonly List<string> args = new List<string>();
		private readonly List<IOptionValue> options = new List<IOptionValue>();

		internal CommandLine(bool parsed) {
			HasParsed = parsed;
		}

		public bool HasParsed { get; private set; }

		public IEnumerable<string> Arguments {
			get { return args.AsReadOnly(); }
		}

		internal void AddArgument(string arg) {
			args.Add(arg);
		}

		internal void AddOption(IOptionValue opt) {
			options.Add(opt);
		}

		public IEnumerator GetEnumerator() {
			return options.GetEnumerator();
		}

		public IEnumerable<IOption> Options {
			get { return options.Select(x => x.Option); }
		}

		public IOptionValue GetOptionValue(string optionName) {
			var option = options.FirstOrDefault(x => x.Option.Name == optionName || x.Option.LongName == optionName);
			if (option == null)
				return null;

			var optionValue = new OptionValue(option.Option);
			foreach (var value in options.Where(x => x.Option.Name == optionName || x.Option.LongName == optionName)) {
				optionValue.AddValues(value.Values);
			}

			return optionValue;
		}
	}
}