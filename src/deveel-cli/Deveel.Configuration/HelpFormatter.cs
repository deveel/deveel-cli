using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Deveel.Configuration {
	public class HelpFormatter : IHelpFormatter {
		public void PrintHelp(Options options, HelpSettings settings, TextWriter writer, bool autoUsage) {		
			if (string.IsNullOrEmpty(settings.CommandLineSyntax))
				throw new InvalidOperationException("Command-line syntax not specified.");

			if (autoUsage) {
				PrintUsage(options, settings, writer);
			} else {
				PrintSimpleUsage(settings, writer);
			}

			if (settings.HasHeader)
				PrintWrapped(settings, writer, settings.Header);

			PrintOptions(settings, writer, settings.Width, options, settings.LeftPadding, settings.DescriptionPadding);

			if (settings.HasFooter)
				PrintWrapped(settings, writer, settings.Footer);
		}

		public void PrintUsage(Options options, HelpSettings settings, TextWriter writer) {
			// initialise the string buffer
			StringBuilder buff = new StringBuilder(settings.SyntaxPrefix)
				.Append(settings.CommandLineSyntax)
				.Append(" ");

			// create a list for processed option groups
			var processedGroups = new List<IOptionGroup>();

			// temp variable

			var optList = new List<IOption>(options.AllOptions);
			optList.Sort(settings.OptionComparer);

			// iterate over the options
			for (int i = 0; i < optList.Count; i++) {
				// get the next Option
				var option = (Option) optList[i];

				// check if the option is part of an OptionGroup
				IOptionGroup group = options.GetOptionGroup(option);

				// if the option is part of a group 
				if (group != null) {
					// and if the group has not already been processed
					if (!processedGroups.Contains(group)) {
						// add the group to the processed list
						processedGroups.Add(group);


						// add the usage clause
						AppendOptionGroup(settings, buff, group);
					}

					// otherwise the option was displayed in the group
					// previously so ignore it.
				}

					// if the Option is not part of an OptionGroup
				else {
					AppendOption(settings, buff, option, option.IsRequired);
				}

				if (i < optList.Count - 1) {
					buff.Append(" ");
				}
			}


			// call printWrapped
			PrintWrapped(settings, writer, buff.ToString().IndexOf(' ') + 1, buff.ToString());
		}

		private void AppendOptionGroup(HelpSettings settings, StringBuilder buff, IOptionGroup group) {
			if (!group.IsRequired) {
				buff.Append("[");
			}

			var optList = new List<IOption>(group.Options);
			optList.Sort(settings.OptionComparer);
			// for each option in the OptionGroup
			for (int i = 0; i < optList.Count; i++) {
				Option option = (Option) optList[i];
				// whether the option is required or not is handled at group level
				AppendOption(settings, buff, option, true);

				if (i < optList.Count - 1) {
					buff.Append(" | ");
				}
			}

			if (!group.IsRequired) {
				buff.Append("]");
			}
		}

		private void AppendOption(HelpSettings settings, StringBuilder buff, Option option, bool required) {
			if (!required) {
				buff.Append("[");
			}

			if (option.Name != null) {
				buff.Append("-").Append(option.Name);
			} else {
				buff.Append("--").Append(option.LongName);
			}

			// if the Option has a value
			if (option.HasArgument()) {
				if (option.HasArgumentName)
					buff.Append(" <").Append(option.ArgumentName).Append(">");
				else if (!String.IsNullOrEmpty(settings.ArgumentName))
					buff.Append(" <").Append(settings.ArgumentName).Append(">");
			}

			// if the Option is not a required option
			if (!required) {
				buff.Append("]");
			}
		}

		public void PrintSimpleUsage(HelpSettings settings, TextWriter pw) {
			int argPos = settings.CommandLineSyntax.IndexOf(' ') + 1;

			PrintWrapped(settings, pw, settings.SyntaxPrefix.Length + argPos, settings.SyntaxPrefix + settings.CommandLineSyntax);
		}

		public void PrintOptions(HelpSettings settings, TextWriter pw,
			int width,
			Options options,
			int leftPad,
			int descPad) {
			StringBuilder sb = new StringBuilder();

			RenderOptions(settings, sb, width, options, leftPad, descPad);
			pw.WriteLine(sb.ToString());
		}

		public void PrintWrapped(HelpSettings settings, TextWriter pw, String text) {
			PrintWrapped(settings, pw, 0, text);
		}

		public void PrintWrapped(HelpSettings settings, TextWriter pw, int nextLineTabStop, String text) {
			StringBuilder sb = new StringBuilder(text.Length);

			RenderWrappedText(settings, sb, settings.Width, nextLineTabStop, text);
			pw.WriteLine(sb.ToString());
		}

		// --------------------------------------------------------------- Protected

		internal StringBuilder RenderOptions(HelpSettings settings, StringBuilder sb, int width, Options options, int leftPad, int descPad) {
			String lpad = createPadding(leftPad);
			String dpad = createPadding(descPad);

			// first create list containing only <lpad>-a,--aaa where 
			// -a is opt and --aaa is long opt; in parallel look for 
			// the longest opt string this list will be then used to 
			// sort options ascending
			int max = 0;
			StringBuilder optBuf;
			ArrayList prefixList = new ArrayList();

			var optList = new List<IOption>(options.AllOptions);
			optList.Sort(settings.OptionComparer);

			foreach (var option in optList) {
				optBuf = new StringBuilder(8);

				if (option.Name == null) {
					optBuf.Append(lpad).Append("   " + settings.LongOptionPrefix).Append(option.LongName);
				} else {
					optBuf.Append(lpad).Append(settings.OptionPrefix).Append(option.Name);

					if (option.HasLongName()) {
						optBuf.Append(',').Append(settings.LongOptionPrefix).Append(option.LongName);
					}
				}

				if (option.HasArgument()) {
					if (option.HasArgumentName()) {
						optBuf.Append(" <").Append(option.ArgumentName).Append(">");
					} else if (!String.IsNullOrEmpty(settings.ArgumentName)) {
						optBuf.Append(" <").Append(settings.ArgumentName).Append(">");
					} else {
						optBuf.Append(' ');
					}
				}

				prefixList.Add(optBuf);
				max = (optBuf.Length > max) ? optBuf.Length : max;
			}

			int x = 0;

			for (int i = 0; i < optList.Count; i++) {
				Option option = (Option) optList[i];
				optBuf = new StringBuilder(prefixList[x++].ToString());

				if (optBuf.Length < max) {
					optBuf.Append(createPadding(max - optBuf.Length));
				}

				optBuf.Append(dpad);

				int nextLineTabStop = max + descPad;

				if (option.Description != null) {
					optBuf.Append(option.Description);
				}

				RenderWrappedText(settings, sb, width, nextLineTabStop, optBuf.ToString());

				if (i < optList.Count - 1) {
					sb.Append(settings.NewLine);
				}
			}

			return sb;
		}

		internal StringBuilder RenderWrappedText(HelpSettings settings, StringBuilder sb,
			int width,
			int nextLineTabStop,
			String text) {
			int pos = findWrapPos(text, width, 0);

			if (pos == -1) {
				sb.Append(rtrim(text));

				return sb;
			}
			sb.Append(rtrim(text.Substring(0, pos))).Append(settings.NewLine);

			if (nextLineTabStop >= width) {
				// stops infinite loop happening
				nextLineTabStop = 1;
			}

			// all following lines must be padded with nextLineTabStop space 
			// characters
			String padding = createPadding(nextLineTabStop);

			while (true) {
				text = padding + text.Substring(pos).Trim();
				pos = findWrapPos(text, width, 0);

				if (pos == -1) {
					sb.Append(text);

					return sb;
				}

				if ((text.Length > width) && (pos == nextLineTabStop - 1)) {
					pos = width;
				}

				sb.Append(rtrim(text.Substring(0, pos))).Append(settings.NewLine);
			}
		}

		internal int findWrapPos(String text, int width, int startPos) {
			int pos = -1;

			// the line ends before the max wrap pos or a new line char found
			if (((pos = text.IndexOf('\n', startPos)) != -1 && pos <= width)
			    || ((pos = text.IndexOf('\t', startPos)) != -1 && pos <= width)) {
				return pos + 1;
			} else if (startPos + width >= text.Length) {
				return -1;
			}


			// look for the last whitespace character before startPos+width
			pos = startPos + width;

			char c;

			while ((pos >= startPos) && ((c = text[pos]) != ' ')
			       && (c != '\n') && (c != '\r')) {
				--pos;
			}

			// if we found it - just return
			if (pos > startPos) {
				return pos;
			}

			// must look for the first whitespace chearacter after startPos 
			// + width
			pos = startPos + width;

			while ((pos <= text.Length) && ((c = text[pos]) != ' ')
			       && (c != '\n') && (c != '\r')) {
				++pos;
			}

			return (pos == text.Length) ? (-1) : pos;
		}

		internal String createPadding(int len) {
			StringBuilder sb = new StringBuilder(len);

			for (int i = 0; i < len; ++i) {
				sb.Append(' ');
			}

			return sb.ToString();
		}

		internal string rtrim(string s) {
			if ((s == null) || (s.Length == 0)) {
				return s;
			}

			int pos = s.Length;

			while ((pos > 0) && Char.IsWhiteSpace(s[pos - 1])) {
				--pos;
			}

			return s.Substring(0, pos);
		}

		private class OptionComparerImpl : IComparer<IOption> {
			public int Compare(IOption opt1, IOption opt2) {
				return String.Compare(opt1.Key(), opt2.Key(), StringComparison.OrdinalIgnoreCase);
			}
		}
	}

}