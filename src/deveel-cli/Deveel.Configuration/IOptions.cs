using System;
using System.Collections.Generic;

namespace Deveel.Configuration {
    public interface IOptions {
        IEnumerable<IOption> AllOptions { get; }

        ICollection<string> RequiredOptions { get; }


        IOption CreateOption();

        IOptions AddOption(IOption option);

        IOptions AddOptionGroup(IOptionGroup group);

        IOption GetOption(string optionNameOrId);

        IOptionGroup GetOptionGroup(IOption option);

        bool HasOption(string optionNameOrId);
    }
}