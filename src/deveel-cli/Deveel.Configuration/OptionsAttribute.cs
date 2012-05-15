using System;

namespace Deveel.Configuration {
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class OptionsAttribute : Attribute {
	}
}