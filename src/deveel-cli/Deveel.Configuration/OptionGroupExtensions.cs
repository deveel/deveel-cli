using System;
using System.Collections.Generic;
using System.Linq;

namespace Deveel.Configuration {
	public static class OptionGroupExtensions {
		public static IEnumerable<string> Names(this IOptionGroup group) {
			return group.Options.Select(x => x.Name);
		}
	}
}