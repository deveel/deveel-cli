using System;

namespace Deveel.Configuration {
	public sealed class OptionBuilder {
		private String longopt;
		private String description;
		private String argName;
		private bool required;
		private int numberOfArgs = Option.Unitialized;
		private OptionType type;
		private bool optionalArg;
		private char valuesep;

		private void Reset() {
			description = null;
			argName = "arg";
			longopt = null;
			type = OptionType.None;
			required = false;
			numberOfArgs = Option.Unitialized;
			optionalArg = false;
			valuesep = (char)0;
		}

		public OptionBuilder WithLongName(String newLongopt) {
			longopt = newLongopt;
			return this;
		}

		public OptionBuilder HasArgument() {
			numberOfArgs = 1;

			return this;
		}

		public OptionBuilder HasArgument(bool hasArg) {
			numberOfArgs = hasArg ? 1 : Option.Unitialized;

			return this;
		}

		public OptionBuilder WithArgumentName(String name) {
			argName = name;

			return this;
		}

		public OptionBuilder IsRequired() {
			required = true;

			return this;
		}

		public OptionBuilder WithValueSeparator(char sep) {
			valuesep = sep;

			return this;
		}

		public OptionBuilder WithValueSeparator() {
			valuesep = '=';

			return this;
		}

		public OptionBuilder IsRequired(bool newRequired) {
			required = newRequired;

			return this;
		}

		public OptionBuilder HasArguments() {
			numberOfArgs = Option.UnlimitedValues;

			return this;
		}

		public OptionBuilder HasArguments(int num) {
			numberOfArgs = num;

			return this;
		}

		public OptionBuilder HasOptionalArg() {
			numberOfArgs = 1;
			optionalArg = true;

			return this;
		}

		public OptionBuilder HasOptionalArgs() {
			numberOfArgs = Option.UnlimitedValues;
			optionalArg = true;

			return this;
		}

		public OptionBuilder HasOptionalArgs(int numArgs) {
			numberOfArgs = numArgs;
			optionalArg = true;

			return this;
		}

		public OptionBuilder WithType(OptionType newType) {
			type = newType;

			return this;
		}

		public OptionBuilder WithDescription(String newDescription) {
			description = newDescription;

			return this;
		}

		public IOption Create(char opt) {
			return Create(opt.ToString());
		}

		public IOption Create() {
			if (longopt == null) {
				Reset();
				throw new ArgumentException("must specify longopt");
			}

			return Create(null);
		}

		public IOption Create(String opt) {
			Option option;
			try {
				// create the option
				option = new Option(opt, description);

				// set the option properties
				option.LongName = longopt;
				option.IsRequired = required;
				option.HasOptionalArgument = optionalArg;
				option.ArgumentCount = numberOfArgs;
				option.Type = type;
				option.ValueSeparator = valuesep;
				option.ArgumentName = argName;
			} finally {
				// reset the OptionBuilder properties
				Reset();
			}

			// return the Option instance
			return option;
		}

		public static OptionBuilder New() {
			return new OptionBuilder();
		}
	}
}