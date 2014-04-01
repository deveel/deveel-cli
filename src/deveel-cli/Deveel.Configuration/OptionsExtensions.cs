using System;

namespace Deveel.Configuration {
    public static class OptionsExtensions {
        public static TOptions AddOption<TOptions>(this TOptions options, string name) where TOptions : IOptions {
            return AddOption<TOptions>(options, name, null);
        }

        public static TOptions AddOption<TOptions>(this TOptions options, string name, string longName) where TOptions : IOptions {
            return AddOption<TOptions>(options, name, longName, false);
        }

        public static TOptions AddOption<TOptions>(this TOptions options, string name, bool hasArg) where TOptions : IOptions {
            return AddOption<TOptions>(options, name, null, hasArg);
        }

        public static TOptions AddOption<TOptions>(this TOptions options, string name, string longName, bool hasArg) where TOptions : IOptions {
            return AddOption<TOptions>(options, name, longName, hasArg, null);
        }

        public static TOptions AddOption<TOptions>(this TOptions options, string name, string longName, string description) where TOptions : IOptions {
            return AddOption<TOptions>(options, name, longName, false, description);
        }

        public static TOptions AddOption<TOptions>(this TOptions options, string name, string longName, bool hasArg, string description) where TOptions : IOptions {
            var option = options.CreateOption();
            option.Name = name;
            option.LongName = longName;
            option.ArgumentCount = hasArg ? 1 : 0;
            option.Description = description;
            return (TOptions) options.AddOption(option);
        }
    }
}