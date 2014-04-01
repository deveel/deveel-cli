using System;
using System.Collections.Generic;
using System.Linq;

namespace Deveel.Configuration {
	[Serializable]
	public class Options : IOptions {
		private readonly IDictionary<string, Option> shortOpts = new Dictionary<string, Option>();
		private readonly IDictionary<string, Option> longOpts = new Dictionary<string, Option>();
		private readonly IList<string> requiredOpts = new List<string>();
		private readonly IDictionary<string, OptionGroup> optionGroups = new Dictionary<string, OptionGroup>();

	    IOption IOptions.CreateOption() {
	        return new Option();
	    }

	    IOptions IOptions.AddOptionGroup(IOptionGroup group) {
	        return AddOptionGroup((OptionGroup) group);
	    }

		public Options AddOptionGroup(OptionGroup group) {
			if (group.IsRequired) {
				foreach (var option in group.Options) {
					requiredOpts.Add(option.Key());
				}
			}

			foreach(var opt in group.Options) {
			    var option = (Option) opt;
			    // an Option cannot be required if it is in an
				// OptionGroup, either the group is required or
				// nothing is required
				option.IsRequired = false;
				AddOption(option);

				optionGroups[option.Key()] = group;
			}

			return this;
		}

		public Options AddOption(string opt, bool hasArg, string description) {
			AddOption(opt, null, hasArg, description);

			return this;
		}

		public Options AddOption(string opt, string longOpt, bool hasArg, String description) {
			AddOption(new Option(opt, longOpt, hasArg, description));

			return this;
		}

	    IOptions IOptions.AddOption(IOption option) {
	        return AddOption((Option) option);
	    }

		public Options AddOption(Option option) {
			String key = option.Key();

			// add it to the long option list
			if (option.HasLongName()) {
				longOpts[option.LongName] = option;
			}

			// if the option is required add it to the required list
			if (option.IsRequired) {
				if (requiredOpts.Contains(key)) {
					requiredOpts.RemoveAt(requiredOpts.IndexOf(key));
				}
				requiredOpts.Add(key);
			}

			shortOpts[key] = option;

			return this;
		}

		public IEnumerable<Option> AllOptions {
			get { return shortOpts.Values.ToList().AsReadOnly(); }
		}

	    IEnumerable<IOption> IOptions.AllOptions {
	        get { return AllOptions.Cast<IOption>(); }
	    }

		public ICollection<string> RequiredOptions {
			get { return requiredOpts; }
		}

	    IOption IOptions.GetOption(string optionNameOrId) {
	        return GetOption(optionNameOrId);
	    }

		public Option GetOption(string opt) {
			opt = Util.StripLeadingHyphens(opt);

			Option option;
			if (shortOpts.TryGetValue(opt, out option))
				return option;

			if (longOpts.TryGetValue(opt, out option))
				return option;

			return null;
		}

		public bool HasOption(String opt) {
			opt = Util.StripLeadingHyphens(opt);

			return shortOpts.ContainsKey(opt) ||
			       longOpts.ContainsKey(opt);
		}

	    IOptionGroup IOptions.GetOptionGroup(IOption option) {
	        return GetOptionGroup((Option) option);
	    }

		public OptionGroup GetOptionGroup(Option opt) {
			OptionGroup group;
			if (optionGroups.TryGetValue(opt.Key(), out group))
				return group;

			return null;
		}
	}
}