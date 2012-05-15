using System;
using System.Collections;

namespace Deveel.Configuration {
	[Serializable]
	public class Option : ICloneable {
		public const int Unitialized = -1;
		public const int UnlimitedValues = -2;

		private readonly string opt;
		private string longOpt;
		private string argName = "arg";
		private string description;
		private bool required;
		private bool optionalArg;
		private int numberOfArgs = Unitialized;
		private OptionType type;

		private ArrayList values = new ArrayList();
		private char valuesep;

		public Option(string opt, string description)
			: this(opt, null, false, description) {
		}

		public Option(string opt, bool hasArg, string description)
			: this(opt, null, hasArg, description) {
		}

		public Option(string opt, string longOpt, bool hasArg, string description) {
			// ensure that the option is valid
			OptionValidator.ValidateOption(opt);

			this.opt = opt;
			this.longOpt = longOpt;

			// if hasArg is set then the number of arguments is 1
			if (hasArg)
				numberOfArgs = 1;

			this.description = description;
		}

		public int Id {
			get { return Key[0]; }
		}

		internal string Key {
			get {
				// if 'opt' is null, then it is a 'long' option
				return opt == null ? longOpt : opt;
			}
		}

		public string Name {
			get { return opt; }
		}

		public OptionType Type {
			get { return type; }
			set { type = value; }
		}

		public string LongName {
			get { return longOpt; }
			set { longOpt = value; }
		}

		public bool HasOptionalArgument {
			get { return optionalArg; }
			set { optionalArg = value; }
		}

		public bool HasLongName {
			get { return longOpt != null; }
		}

		public bool HasArgument {
			get { return numberOfArgs > 0 || numberOfArgs == UnlimitedValues; }
		}

		public string Description {
			get { return description; }
			set { description = value; }
		}

		public bool IsRequired {
			get { return required; }
			set { required = value; }
		}

		public string ArgumentName {
			set { argName = value; }
			get { return argName; }
		}

		public bool HasArgumentName {
			get { return argName != null && argName.Length > 0; }
		}

		public bool HasMultipleArguments {
			get { return numberOfArgs > 1 || numberOfArgs == UnlimitedValues; }
		}

		public int ArgumentCount {
			set { numberOfArgs = value; }
			get { return numberOfArgs; }
		}

		public char ValueSeparator {
			set { valuesep = value; }
			get { return valuesep; }
		}

		public bool HasValueSeparator {
			get { return valuesep > 0; }
		}

		internal void AddValueForProcessing(String value) {
			if (numberOfArgs == Unitialized)
				throw new ApplicationException("NO_ARGS_ALLOWED");

			ProcessValue(value);
		}

		private void ProcessValue(String value) {
			// this Option has a separator character
			if (HasValueSeparator) {
				// get the separator character
				char sep = ValueSeparator;

				// store the index for the value separator
				int index = value.IndexOf(sep);

				// while there are more value separators
				while (index != -1) {
					// next value to be added 
					if (values.Count == (numberOfArgs - 1)) {
						break;
					}

					// store
					Add(value.Substring(0, index));

					// parse
					value = value.Substring(index + 1);

					// get new index
					index = value.IndexOf(sep);
				}
			}

			// store the actual value or the last value that has been parsed
			Add(value);
		}

		private void Add(String value) {
			if ((numberOfArgs > 0) && (values.Count > (numberOfArgs - 1))) {
				throw new ApplicationException("Cannot add value, list full.");
			}

			// store value
			values.Add(value);
		}

		public string Value {
			get { return HasNoValues ? null : (String) values[0]; }
		}

		public String GetValue(int index) {
			return HasNoValues ? null : (String)values[index];
		}

		public String GetValue(String defaultValue) {
			String value = Value;

			return (value != null) ? value : defaultValue;
		}

		public string[] Values {
			get { return HasNoValues ? null : (string[]) values.ToArray(typeof(string)); }
		}

		public IList ValuesList {
			get { return ArrayList.ReadOnly(values); }
		}

		private bool HasNoValues {
			get { return values.Count == 0; }
		}

		public override bool Equals(Object o) {
			if (this == o) {
				return true;
			}
			if (o == null || GetType() != o.GetType()) {
				return false;
			}

			Option option = (Option)o;


			if (opt != null ? !opt.Equals(option.opt) : option.opt != null) {
				return false;
			}
			if (longOpt != null ? !longOpt.Equals(option.longOpt) : option.longOpt != null) {
				return false;
			}

			return true;
		}

		public override int GetHashCode() {
			int result;
			result = (opt != null ? opt.GetHashCode() : 0);
			result = 31 * result + (longOpt != null ? longOpt.GetHashCode() : 0);
			return result;
		}

		public Object Clone() {
			Option option = (Option)base.MemberwiseClone();
			option.values = new ArrayList(values);
			return option;
		}

		internal void ClearValues() {
			values.Clear();
		}
	}
}