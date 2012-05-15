using System;
using System.Collections;
using System.Collections.Generic;

namespace Deveel.Configuration {
	[Serializable]
	public class CommandLine : IEnumerable {
		private readonly List<string> args = new List<string>();
		private readonly List<Option> options = new List<Option>();
		private bool parsed;

		internal CommandLine(bool parsed) {
			this.parsed = parsed;
		}
		
		public bool HasParsed {
			get { return parsed; }
		}

		public bool HasOption(String opt) {
			return options.Contains(ResolveOption(opt));
		}

		public bool HasOption(char opt) {
			return HasOption(opt.ToString());
		}

		public object GetOptionObject(String opt) {
			try {
				return GetParsedOptionValue(opt);
			} catch (ParseException pe) {
				Console.Error.WriteLine("Exception found converting " + opt + " to desired type: " + pe.Message);
				return null;
			}
		}

		public Object GetParsedOptionValue(String opt) {
			String res = GetOptionValue(opt);

			Option option = ResolveOption(opt);
			if (option == null) {
				return null;
			}

			OptionType type = option.Type;

			return (res == null) ? null : TypeHandler.CreateValue(res, type);
		}

		public Object GetOptionObject(char opt) {
			return GetOptionObject(opt.ToString());
		}

		public String GetOptionValue(String opt) {
			String[] values = GetOptionValues(opt);

			return (values == null) ? null : values[0];
		}

		public string GetOptionValue(char opt) {
			return GetOptionValue(opt.ToString());
		}

		public String[] GetOptionValues(String opt) {
			ArrayList values = new ArrayList();

			foreach(Option option in options) {
				if (opt.Equals(option.Name) || 
					opt.Equals(option.LongName)) {
					values.AddRange(option.ValuesList);
				}
			}

			return values.Count == 0 ? null : (String[])values.ToArray(typeof(String));
		}

		private Option ResolveOption(String opt) {
			opt = Util.StripLeadingHyphens(opt);
			foreach(Option option in options) {
				if (opt.Equals(option.Name))
					return option;

				if (opt.Equals(option.LongName))
					return option;

			}
			return null;
		}

		public String[] GetOptionValues(char opt) {
			return GetOptionValues(opt.ToString());
		}

		public String GetOptionValue(string opt, String defaultValue) {
			String answer = GetOptionValue(opt);

			return (answer != null) ? answer : defaultValue;
		}

		public String GetOptionValue(char opt, String defaultValue) {
			return GetOptionValue(opt.ToString(), defaultValue);
		}

		public IDictionary GetOptionProperties(String opt) {
			Hashtable props = new Hashtable();

			foreach(Option option in options) {
				if (opt.Equals(option.Name) || 
					opt.Equals(option.LongName)) {
					IList values = option.ValuesList;
					if (values.Count >= 2) {
						// use the first 2 arguments as the key/value pair
						props[values[0]] = values[1];
					} else if (values.Count == 1) {
						// no explicit value, handle it as a bool
						props[values[0]] = "true";
					}
				}
			}

			return props;
		}

		public string[] Arguments {
			get {
				string[] answer = new string[args.Count];
				args.CopyTo(answer, 0);
				return answer;
			}
		}

		public IList ArgumentList {
			get { return ArrayList.ReadOnly(args); }
		}

		internal void AddArgument(String arg) {
			args.Add(arg);
		}

		internal void AddOption(Option opt) {
			options.Add(opt);
		}

		public IEnumerator GetEnumerator() {
			return options.GetEnumerator();
		}

		public Option[] Options {
			get {
				Option[] optionsArray = new Option[options.Count];
				options.CopyTo(optionsArray, 0);
				return optionsArray;
			}
		}
	}
}