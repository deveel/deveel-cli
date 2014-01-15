using System;
using System.Collections.Generic;

namespace Deveel.Configuration {
	public interface IOptionValue : ICloneable {
		IOption Option { get; }

		string Value { get; }

		IEnumerable<string> Values { get; }
	}
}