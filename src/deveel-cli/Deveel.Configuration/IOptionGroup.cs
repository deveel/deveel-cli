using System;
using System.Collections.Generic;

namespace Deveel.Configuration {
	public interface IOptionGroup {
		IEnumerable<IOption> Options { get; }

		bool IsRequired { get; }
	}
}