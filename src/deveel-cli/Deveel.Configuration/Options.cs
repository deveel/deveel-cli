using System;
using System.Collections;

namespace Deveel.Configuration {
	[Serializable]
	public class Options {
		private readonly IDictionary shortOpts = new Hashtable();
		private readonly IDictionary longOpts = new Hashtable();
		private readonly IList requiredOpts = new ArrayList();
		private readonly IDictionary optionGroups = new Hashtable();

		public Options AddOptionGroup(OptionGroup group) {
			if (group.IsRequired)
				requiredOpts.Add(group);

			foreach(Option option in group.Options) {
				// an Option cannot be required if it is in an
				// OptionGroup, either the group is required or
				// nothing is required
				option.IsRequired = false;
				AddOption(option);

				optionGroups[option.Key] = group;
			}

			return this;
		}

		internal ICollection OptionGroups {
			get { return new ArrayList(optionGroups.Values); }
		}

		public Options AddOption(string opt, bool hasArg, string description) {
			AddOption(opt, null, hasArg, description);

			return this;
		}

		public Options AddOption(string opt, string longOpt, bool hasArg, String description) {
			AddOption(new Option(opt, longOpt, hasArg, description));

			return this;
		}

		public Options AddOption(Option opt) {
			String key = opt.Key;

			// add it to the long option list
			if (opt.HasLongName) {
				longOpts[opt.LongName] = opt;
			}

			// if the option is required add it to the required list
			if (opt.IsRequired) {
				if (requiredOpts.Contains(key)) {
					requiredOpts.RemoveAt(requiredOpts.IndexOf(key));
				}
				requiredOpts.Add(key);
			}

			shortOpts[key] = opt;

			return this;
		}

		public ICollection getOptions() {
			return ArrayList.ReadOnly(HelpOptions);
		}

		internal IList HelpOptions {
			get { return new ArrayList(shortOpts.Values); }
		}

		public IList RequiredOptions {
			get { return requiredOpts; }
		}

		public Option GetOption(String opt) {
			opt = Util.StripLeadingHyphens(opt);

			if (shortOpts.Contains(opt)) {
				return (Option)shortOpts[opt];
			}

			return (Option)longOpts[opt];
		}

		public bool HasOption(String opt) {
			opt = Util.StripLeadingHyphens(opt);

			return shortOpts.Contains(opt) ||
			       longOpts.Contains(opt);
		}

		public OptionGroup GetOptionGroup(Option opt) {
			return (OptionGroup)optionGroups[opt.Key];
		}
	}
}