using System;

namespace Deveel.Configuration {
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false)]
	public sealed class OptionGroupAttribute : Attribute {
		public OptionGroupAttribute(string name) {
			Name = name;
		}

		public string Name { get; private set; }

		public bool IsRequired { get; set; }
	}
}