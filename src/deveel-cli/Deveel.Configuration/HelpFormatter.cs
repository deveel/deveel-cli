using System;
using System.Collections;
using System.IO;
using System.Text;

namespace Deveel.Configuration {
	public class HelpFormatter {
		
		public const int DEFAULT_WIDTH = 74;
		public const int DEFAULT_LEFT_PAD = 1;
		public const int DEFAULT_DESC_PAD = 3;
		
		public const String DEFAULT_SYNTAX_PREFIX = "usage: ";
		public const String DEFAULT_OPT_PREFIX = "-";
		public const String DEFAULT_LONG_OPT_PREFIX = "--";
		public const String DEFAULT_ARG_NAME = "arg";
		
		private int defaultWidth = DEFAULT_WIDTH;
		private int defaultLeftPad = DEFAULT_LEFT_PAD;
		private int defaultDescPad = DEFAULT_DESC_PAD;
		
		private string defaultSyntaxPrefix = DEFAULT_SYNTAX_PREFIX;
		private string defaultNewLine = Environment.NewLine;
		private string defaultOptPrefix = DEFAULT_OPT_PREFIX;
		private string defaultLongOptPrefix = DEFAULT_LONG_OPT_PREFIX;
		private string defaultArgName = DEFAULT_ARG_NAME;
		
		private Options options;
		private string header;
		private string footer;
		private string cmdLineSyntax;
		
		protected IComparer optionComparator = new OptionComparerImpl();
		
		public int Width {
			get { return defaultWidth; }
			set { defaultWidth = value; }
		}
		
		public int LeftPadding {
			get { return defaultLeftPad; }
			set { this.defaultLeftPad = value; }
		}
		
		public int DescriptionPadding {
			get { return defaultDescPad; }
			set { this.defaultDescPad = value; }
		}
		
		public string SyntaxPrefix {
			set { this.defaultSyntaxPrefix = value; }
			get { return defaultSyntaxPrefix; }
		}
		
		public string NewLine {
			get { return defaultNewLine; }
			set { this.defaultNewLine = value; }
		}
		
		public string OptionPrefix {
			set { this.defaultOptPrefix = value; }
			get { return defaultOptPrefix; }
		}
		
		public string LongOptionPrefix {
			get { return defaultLongOptPrefix; }
			set { this.defaultLongOptPrefix = value; }
		}
		
		public string ArgumentName {
			get { return defaultArgName; }
			set { this.defaultArgName = value; }
		}
		
		public string Header {
			get { return header; }
			set { header = value; }
		}
		
		public string Footer {
			get { return footer; }
			set { footer = value; }
		}
		
		public string CommandLineSyntax {
			get { return cmdLineSyntax; }
			set { cmdLineSyntax = value; }
		}
				
		public IComparer OptionComparer {
			get { return optionComparator; }
			set {
				if (value == null) {
					this.optionComparator = new OptionComparerImpl();
				} else {
					this.optionComparator = value;
				}
			}
		}
		
		public Options Options {
			get { return options; }
			set { options = value; }
		}
		
		public void PrintHelp() {
			PrintHelp(false);
		}
				
		public void PrintHelp(bool autoUsage) {
			TextWriter pw = Console.Out;
			PrintHelp(pw, autoUsage);
			pw.Flush();
		}
				
		public void PrintHelp(TextWriter pw, bool autoUsage) {
			if (cmdLineSyntax == null || cmdLineSyntax.Length == 0)
				throw new InvalidOperationException("Command-line syntax not specified.");
			
			if (autoUsage) {
				PrintUsage(pw);
			} else {
				PrintSimpleUsage(pw);
			}
			
			if (header != null && header.Trim().Length > 0)
				printWrapped(pw, header);
			
			printOptions(pw, defaultWidth, options, defaultLeftPad, defaultDescPad);
			
			if (footer != null && footer.Trim().Length > 0)
				printWrapped(pw, footer);
		}
		
		public void PrintUsage(TextWriter pw) {
			// initialise the string buffer
			StringBuilder buff = new StringBuilder(defaultSyntaxPrefix).Append(cmdLineSyntax).Append(" ");
			
			// create a list for processed option groups
			ArrayList processedGroups = new ArrayList();
			
			// temp variable
			Option option;
			
			ArrayList optList = new ArrayList(options.getOptions());
			optList.Sort(OptionComparer);
			
			// iterate over the options
			for (int i = 0; i < optList.Count; i++) {
				// get the next Option
				option = (Option)optList[i];

            // check if the option is part of an OptionGroup
            OptionGroup group = options.GetOptionGroup(option);

            // if the option is part of a group 
            if (group != null)
            {
                // and if the group has not already been processed
                if (!processedGroups.Contains(group))
                {
                    // add the group to the processed list
                    processedGroups.Add(group);


                    // add the usage clause
                    appendOptionGroup(buff, group);
                }

                // otherwise the option was displayed in the group
                // previously so ignore it.
            }

            // if the Option is not part of an OptionGroup
            else
            {
                appendOption(buff, option, option.IsRequired);
            }

            if (i < optList.Count - 1)
            {
                buff.Append(" ");
            }
        }


        // call printWrapped
        printWrapped(pw, buff.ToString().IndexOf(' ') + 1, buff.ToString());
    }

    private void appendOptionGroup(StringBuilder buff, OptionGroup group)
    {
        if (!group.IsRequired)
        {
            buff.Append("[");
        }

        ArrayList optList = new ArrayList(group.Options);
        optList.Sort(OptionComparer);
        // for each option in the OptionGroup
        for (int i = 0; i < optList.Count; i++) {
        	Option option = (Option) optList[i];
            // whether the option is required or not is handled at group level
            appendOption(buff, option, true);

            if (i < optList.Count - 1)
            {
                buff.Append(" | ");
            }
        }

        if (!group.IsRequired)
        {
            buff.Append("]");
        }
    }

    private void appendOption(StringBuilder buff, Option option, bool required)
    {
        if (!required)
        {
            buff.Append("[");
        }

        if (option.Name != null)
        {
            buff.Append("-").Append(option.Name);
        }
        else
        {
            buff.Append("--").Append(option.LongName);
        }

        // if the Option has a value
        if (option.HasArgument)
        {
        	if (option.HasArgumentName)
            	buff.Append(" <").Append(option.ArgumentName).Append(">");
        	else if (!String.IsNullOrEmpty(defaultArgName))
        		buff.Append(" <").Append(defaultArgName).Append(">");
        }

        // if the Option is not a required option
        if (!required)
        {
            buff.Append("]");
        }
    }

    public void PrintSimpleUsage(TextWriter pw)
    {
        int argPos = cmdLineSyntax.IndexOf(' ') + 1;

        printWrapped(pw, defaultSyntaxPrefix.Length + argPos, defaultSyntaxPrefix + cmdLineSyntax);
    }

    public void printOptions(TextWriter pw, int width, Options options, 
                             int leftPad, int descPad)
    {
        StringBuilder sb = new StringBuilder();

        renderOptions(sb, width, options, leftPad, descPad);
        pw.WriteLine(sb.ToString());
    }

    public void printWrapped(TextWriter pw, String text)
    {
        printWrapped(pw, 0, text);
    }

    public void printWrapped(TextWriter pw, int nextLineTabStop, String text)
    {
        StringBuilder sb = new StringBuilder(text.Length);

        renderWrappedText(sb, defaultWidth, nextLineTabStop, text);
        pw.WriteLine(sb.ToString());
    }

    // --------------------------------------------------------------- Protected

    internal StringBuilder renderOptions(StringBuilder sb, int width, Options options, int leftPad, int descPad)
    {
        String lpad = createPadding(leftPad);
        String dpad = createPadding(descPad);

        // first create list containing only <lpad>-a,--aaa where 
        // -a is opt and --aaa is long opt; in parallel look for 
        // the longest opt string this list will be then used to 
        // sort options ascending
        int max = 0;
        StringBuilder optBuf;
        ArrayList prefixList = new ArrayList();

        ArrayList optList = new ArrayList(options.HelpOptions);

        optList.Sort(OptionComparer);

        for (IEnumerator i = optList.GetEnumerator(); i.MoveNext();)
        {
            Option option = (Option) i.Current;
            optBuf = new StringBuilder(8);

            if (option.Name == null)
            {
                optBuf.Append(lpad).Append("   " + defaultLongOptPrefix).Append(option.LongName);
            }
            else
            {
                optBuf.Append(lpad).Append(defaultOptPrefix).Append(option.Name);

                if (option.HasLongName)
                {
                    optBuf.Append(',').Append(defaultLongOptPrefix).Append(option.LongName);
                }
            }

            if (option.HasArgument)
            {
                if (option.HasArgumentName)
                {
                    optBuf.Append(" <").Append(option.ArgumentName).Append(">");
                } else if (!String.IsNullOrEmpty(defaultArgName)) {
                	optBuf.Append(" <").Append(defaultArgName).Append(">");
                }
                else
                {
                    optBuf.Append(' ');
                }
            }

            prefixList.Add(optBuf);
            max = (optBuf.Length > max) ? optBuf.Length : max;
        }

        int x = 0;

        for (int i = 0; i < optList.Count; i++) {
        	Option option = (Option)optList[i];
            optBuf = new StringBuilder(prefixList[x++].ToString());

            if (optBuf.Length < max)
            {
                optBuf.Append(createPadding(max - optBuf.Length));
            }

            optBuf.Append(dpad);

            int nextLineTabStop = max + descPad;

            if (option.Description != null)
            {
                optBuf.Append(option.Description);
            }

            renderWrappedText(sb, width, nextLineTabStop, optBuf.ToString());

            if (i < optList.Count - 1)
            {
                sb.Append(defaultNewLine);
            }
        }

        return sb;
    }

    internal StringBuilder renderWrappedText(StringBuilder sb, int width, 
                                             int nextLineTabStop, String text)
    {
        int pos = findWrapPos(text, width, 0);

        if (pos == -1)
        {
            sb.Append(rtrim(text));

            return sb;
        }
        sb.Append(rtrim(text.Substring(0, pos))).Append(defaultNewLine);

        if (nextLineTabStop >= width)
        {
            // stops infinite loop happening
            nextLineTabStop = 1;
        }

        // all following lines must be padded with nextLineTabStop space 
        // characters
        String padding = createPadding(nextLineTabStop);

        while (true)
        {
            text = padding + text.Substring(pos).Trim();
            pos = findWrapPos(text, width, 0);

            if (pos == -1)
            {
                sb.Append(text);

                return sb;
            }
            
            if ( (text.Length > width) && (pos == nextLineTabStop - 1) ) 
            {
                pos = width;
            }

            sb.Append(rtrim(text.Substring(0, pos))).Append(defaultNewLine);
        }
    }

    internal int findWrapPos(String text, int width, int startPos)
    {
        int pos = -1;

        // the line ends before the max wrap pos or a new line char found
        if (((pos = text.IndexOf('\n', startPos)) != -1 && pos <= width)
                || ((pos = text.IndexOf('\t', startPos)) != -1 && pos <= width))
        {
            return pos + 1;
        }
        else if (startPos + width >= text.Length)
        {
            return -1;
        }


        // look for the last whitespace character before startPos+width
        pos = startPos + width;

        char c;

        while ((pos >= startPos) && ((c = text[pos]) != ' ')
                && (c != '\n') && (c != '\r'))
        {
            --pos;
        }

        // if we found it - just return
        if (pos > startPos)
        {
            return pos;
        }
        
        // must look for the first whitespace chearacter after startPos 
        // + width
        pos = startPos + width;

        while ((pos <= text.Length) && ((c = text[pos]) != ' ')
               && (c != '\n') && (c != '\r'))
        {
            ++pos;
        }

        return (pos == text.Length) ? (-1) : pos;
    }

    internal String createPadding(int len)
    {
        StringBuilder sb = new StringBuilder(len);

        for (int i = 0; i < len; ++i)
        {
            sb.Append(' ');
        }

        return sb.ToString();
    }

    internal string rtrim(string s)
    {
        if ((s == null) || (s.Length == 0))
        {
            return s;
        }

        int pos = s.Length;

        while ((pos > 0) && Char.IsWhiteSpace(s[pos - 1]))
        {
            --pos;
        }

        return s.Substring(0, pos);
    }

    private class OptionComparerImpl : IComparer
    {

        /**
         * Compares its two arguments for order. Returns a negative
         * integer, zero, or a positive integer as the first argument
         * is less than, equal to, or greater than the second.
         *
         * @param o1 The first Option to be compared.
         * @param o2 The second Option to be compared.
         * @return a negative integer, zero, or a positive integer as
         *         the first argument is less than, equal to, or greater than the
         *         second.
         */
        public int Compare(Object o1, Object o2)
        {
            Option opt1 = (Option) o1;
            Option opt2 = (Option) o2;

            return String.Compare(opt1.Key, opt2.Key, true);
        }
    }
}

}