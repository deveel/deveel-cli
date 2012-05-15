using System;

namespace Deveel.Configuration {
	public class PatternOptionBuilder {

		public static OptionType getValueClass(char ch) {
			switch (ch) {
				case '@': return OptionType.Object;
				case ':': return OptionType.String;
				case '%': return OptionType.Number;
				case '+': return OptionType.Type;
				case '#': return OptionType.Date;
				case '<': return OptionType.ExistingFile;
				case '>': return OptionType.File;
				case '*': return OptionType.FileList;
				case '/': return OptionType.Url;
			}

			return OptionType.None;
		}

		public static bool IsValueCode(char ch) {
			return ch == '@' ||
			       ch == ':' ||
			       ch == '%' ||
			       ch == '+' ||
			       ch == '#' ||
			       ch == '<' ||
			       ch == '>' ||
			       ch == '*' ||
			       ch == '/' ||
			       ch == '!';
		}

		public static Options Parse(String pattern) {
			char opt = ' ';
			bool required = false;
			OptionType type = OptionType.None;

			Options options = new Options();

			for (int i = 0; i < pattern.Length; i++) {
				char ch = pattern[i];

				// a value code comes after an option and specifies
				// details about it
				if (!IsValueCode(ch)) {
					if (opt != ' ') {
						OptionBuilder builder = new OptionBuilder();
						builder.HasArgument(type != OptionType.None);
						builder.IsRequired(required);
						builder.WithType(type);

						// we have a previous one to deal with
						options.AddOption(builder.Create(opt));
						required = false;
						type = OptionType.None;
						opt = ' ';
					}

					opt = ch;
				} else if (ch == '!') {
					required = true;
				} else {
					type = getValueClass(ch);
				}
			}

			if (opt != ' ') {
				OptionBuilder builder = new OptionBuilder();
				builder.HasArgument(type != OptionType.None);
				builder.IsRequired(required);
				builder.WithType(type);

				// we have a final one to deal with
				options.AddOption(builder.Create(opt));
			}

			return options;
		}
	}
}