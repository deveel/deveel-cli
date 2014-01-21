using System;

namespace Deveel.Configuration {
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
	public sealed class OptionAttribute : Attribute {
		private bool optionalArg;
		private int argumentCount;

		public OptionAttribute(string name, string longName, bool required) {
			Name = name;
			LongName = longName;
			IsRequired = required;
		}
		
		public OptionAttribute(string name, bool required)
			: this(name, null, required) {
		}
		
		public OptionAttribute(bool required)
			: this(null, required) {
		}
		
		public OptionAttribute(string name, string longName)
			: this(name, longName, false) {
		}
		
		public OptionAttribute(string name)
			: this(name, null) {
		}
		
		public OptionAttribute()
			: this(null) {
		}

		public string Name { get; set; }

		public string LongName { get; set; }

		public bool IsRequired { get; set; }

		public int ArgumentCount {
			get { return argumentCount; }
			set { 
				argumentCount = value;
				if (argumentCount > 0)
					optionalArg = true;
			}
		}
		
		public bool HasOptionalArgument {
			get { return optionalArg; }
			set {
				optionalArg = value;
				if (optionalArg && argumentCount <= 0)
					argumentCount = 1;
			}
		}

		public string Description { get; set; }

		public string ArgumentName { get; set; }

		public char ValueSeparator { get; set; }
	}
}