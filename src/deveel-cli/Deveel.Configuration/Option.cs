using System;

namespace Deveel.Configuration {
	[Serializable]
	public class Option : IOption {
		public const int Unitialized = -1;
		public const int UnlimitedValues = -2;
		public const int OptionalArguments = -3;

	    private bool optionalArg;

	    public Option(string opt)
	        : this(opt, null) {
	    }

	    public Option(string opt, string description)
			: this(opt, null, false, description) {
		}

	    public Option(string opt, bool hasArg)
	        : this(opt, hasArg, null) {
	    }

	    public Option(string opt, bool hasArg, string description)
			: this(opt, null, hasArg, description) {
		}

	    public Option(string opt, string longOpt, string description) 
            : this(opt, longOpt, false, description) {
	    }

	    public Option(string opt, string longOpt, bool hasArg, string description) {
	        ArgumentName = "arg";
	        ArgumentCount = Unitialized;
			// ensure that the option is valid
			OptionValidator.ValidateOption(opt);

			Name = opt;
			LongName = longOpt;

			// if hasArg is set then the number of arguments is 1
			if (hasArg)
				ArgumentCount = 1;

			Description = description;
		}

	    public Option() {
	        ArgumentName = "arg";
	    }

	    public int Id {
			get { return this.Key()[0]; }
		}

		public string Name { get; set; }

		public OptionType Type { get; set; }

		public string LongName { get; set; }

		public bool HasOptionalArgument {
			get { return optionalArg; }
			set { optionalArg = value; }
		}

		public string Description { get; set; }

		public bool IsRequired { get; set; }

	    public string ArgumentName { get; set; }

	    public bool HasArgumentName {
			get { return !string.IsNullOrEmpty(ArgumentName); }
		}

		public int ArgumentCount { get; set; }

		public char ValueSeparator { get; set; }

		public bool Equals(IOption other) {
			if (Name != null ? !Name.Equals(other.Name) : other.Name != null)
				return false;

			if (LongName != null ? !LongName.Equals(other.LongName) : other.LongName != null)
				return false;

			return true;
		}

		public override bool Equals(object o) {
			if (this == o)
				return true;
			if (o == null || GetType() != o.GetType())
				return false;

			var option = (Option)o;

			return Equals(option);
		}

		public override int GetHashCode() {
			int result;
			result = (Name != null ? Name.GetHashCode() : 0);
			result = 31 * result + (LongName != null ? LongName.GetHashCode() : 0);
			return result;
		}
	}
}