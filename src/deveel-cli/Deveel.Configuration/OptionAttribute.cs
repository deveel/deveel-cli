using System;

namespace Deveel.Configuration {
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
	public sealed class OptionAttribute : Attribute {
		private string name;
		private string longName;
		private bool required;
		private bool optionalArg;
		private string description;
		private int argumentCount;
		private string argName;
		private char valueSep;
		
		public OptionAttribute(string name, string longName, bool required) {
			this.name = name;
			this.longName = longName;
			this.required = required;
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
		
		public string Name {
			get { return name; }
			set { name = value; }
		}
		
		public string LongName {
			get { return longName; }
			set { longName = value; }
		}
		
		public bool IsRequired {
			get { return required; }
			set { required = value; }
		}
		
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
		
		public string Description {
			get { return description; }
			set { description = value; }
		}
		
		public string ArgumentName {
			get { return argName; }
			set { argName = value; }
		}
		
		public char ValueSeparator {
			get { return valueSep; }
			set { valueSep = value; }
		}
	}
}