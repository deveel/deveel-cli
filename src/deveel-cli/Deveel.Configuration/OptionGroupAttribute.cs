using System;

namespace Deveel.Configuration {
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false)]
	public sealed class OptionGroupAttribute : Attribute {
		private readonly string name;
		private bool required;
		
		public OptionGroupAttribute(string name) {
			this.name = name;
		}
		
		public string Name {
			get { return name; }
		}
		
		public bool IsRequired {
			get { return required; }
			set { required = value; }
		}
	}
}