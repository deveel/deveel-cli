using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Remoting.Messaging;

namespace Deveel.Configuration {
	[Serializable]
	public class Options {
		private readonly IDictionary<string, IOption> shortOpts = new Dictionary<string, IOption>();
		private readonly IDictionary longOpts = new Hashtable();
		private readonly IList<string> requiredOpts = new List<string>();
		private readonly IDictionary<string, IOptionGroup> optionGroups = new Dictionary<string, IOptionGroup>();

		public Options AddOptionGroup(IOptionGroup group) {
			if (group.IsRequired) {
				foreach (var option in group.Options) {
					requiredOpts.Add(option.Key());
				}
			}

			foreach(Option option in group.Options) {
				// an Option cannot be required if it is in an
				// OptionGroup, either the group is required or
				// nothing is required
				option.IsRequired = false;
				AddOption(option);

				optionGroups[option.Key()] = group;
			}

			return this;
		}

		internal IEnumerable<IOptionGroup> OptionGroups {
			get { return optionGroups.Values.ToList().AsReadOnly(); }
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
			String key = opt.Key();

			// add it to the long option list
			if (opt.HasLongName()) {
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

		public IEnumerable<IOption> getOptions() {
			return new ReadOnlyCollection<IOption>(HelpOptions);
		}

		internal IList<IOption> HelpOptions {
			get { return new List<IOption>(shortOpts.Values); }
		}

		public IList<string> RequiredOptions {
			get { return requiredOpts; }
		}

		public Option GetOption(String opt) {
			opt = Util.StripLeadingHyphens(opt);

			if (shortOpts.ContainsKey(opt)) {
				return (Option)shortOpts[opt];
			}

			return (Option)longOpts[opt];
		}

		public bool HasOption(String opt) {
			opt = Util.StripLeadingHyphens(opt);

			return shortOpts.ContainsKey(opt) ||
			       longOpts.Contains(opt);
		}

		public OptionGroup GetOptionGroup(IOption opt) {
			IOptionGroup group;
			if (optionGroups.TryGetValue(opt.Key(), out group))
				return (OptionGroup) group;

			return null;
		}
	}
}