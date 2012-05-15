using System;
using NUnit.Framework;

namespace Deveel.Configuration {
	[TestFixture]
	public class ApplicationTest {

		[Test]
		public void Ls() {
			Options options = new Options();
			options.AddOption("a", "all", false, "do not hide entries starting with .");
			options.AddOption("A", "almost-all", false, "do not list implied . and ..");
			options.AddOption("b", "escape", false, "print octal escapes for nongraphic characters");
			options.AddOption(OptionBuilder.New().WithLongName("block-size")
											.WithDescription("use SIZE-byte blocks")
											.HasArgument()
											.WithArgumentName("SIZE")
											.Create());
			options.AddOption("B", "ignore-backups", false, "do not list implied entried ending with ~");
			options.AddOption("c", false, "with -lt: sort by, and show, ctime (time of last modification of file status information) with -l:show ctime and sort by name otherwise: sort by ctime");
			options.AddOption("C", false, "list entries by columns");

			String[] args = new String[] { "--block-size=10" };

			// create the command line parser
			ICommandLineParser parser = new PosixParser(options);

			CommandLine line = parser.Parse(args);
			Assert.IsTrue(line.HasOption("block-size"));
			Assert.AreEqual(line.GetOptionValue("block-size"), "10");
		}

		[Test]
		public void Ant() {
			Options options = new Options();
			options.AddOption("help", false, "print this message");
			options.AddOption("projecthelp", false, "print project help information");
			options.AddOption("version", false, "print the version information and exit");
			options.AddOption("quiet", false, "be extra quiet");
			options.AddOption("verbose", false, "be extra verbose");
			options.AddOption("debug", false, "print debug information");
			options.AddOption("logfile", true, "use given file for log");
			options.AddOption("logger", true, "the class which is to perform the logging");
			options.AddOption("listener", true, "add an instance of a class as a project listener");
			options.AddOption("buildfile", true, "use given buildfile");
			options.AddOption(OptionBuilder.New().WithDescription("use value for given property")
											.HasArguments()
											.WithValueSeparator()
											.Create('D'));
			//, null, true, , false, true );
			options.AddOption("find", true, "search for buildfile towards the root of the filesystem and use it");

			String[] args = new String[]{ "-buildfile", "mybuild.xml",
            "-Dproperty=value", "-Dproperty1=value1",
            "-projecthelp" };
			
			// use the GNU parser
			ICommandLineParser parser = new GnuParser(options);

			CommandLine line = parser.Parse(args);

			// check multiple values
			String[] opts = line.GetOptionValues("D");
			Assert.AreEqual("property", opts[0]);
			Assert.AreEqual("value", opts[1]);
			Assert.AreEqual("property1", opts[2]);
			Assert.AreEqual("value1", opts[3]);

			// check single value
			Assert.AreEqual(line.GetOptionValue("buildfile"), "mybuild.xml");

			// check option
			Assert.IsTrue(line.HasOption("projecthelp"));
		}

		[Test]
		public void Groovy() {
			Options options = new Options();

			options.AddOption(
				OptionBuilder.New().WithLongName("define").
					WithDescription("define a system property").
					HasArgument(true).
					WithArgumentName("name=value").
					Create('D'));
			options.AddOption(
				OptionBuilder.New().HasArgument(false)
				.WithDescription("usage information")
				.WithLongName("help")
				.Create('h'));
			options.AddOption(
				OptionBuilder.New().HasArgument(false)
				.WithDescription("debug mode will print out full stack traces")
				.WithLongName("debug")
				.Create('d'));
			options.AddOption(
				OptionBuilder.New().HasArgument(false)
				.WithDescription("display the Groovy and JVM versions")
				.WithLongName("version")
				.Create('v'));
			options.AddOption(
				OptionBuilder.New().WithArgumentName("charset")
				.HasArgument()
				.WithDescription("specify the encoding of the files")
				.WithLongName("encoding")
				.Create('c'));
			options.AddOption(
				OptionBuilder.New().WithArgumentName("script")
				.HasArgument()
				.WithDescription("specify a command line script")
				.Create('e'));
			options.AddOption(
				OptionBuilder.New().WithArgumentName("extension")
				.hasOptionalArg()
				.WithDescription("modify files in place; create backup if extension is given (e.g. \'.bak\')")
				.Create('i'));
			options.AddOption(
				OptionBuilder.New().HasArgument(false)
				.WithDescription("process files line by line using implicit 'line' variable")
				.Create('n'));
			options.AddOption(
				OptionBuilder.New().HasArgument(false)
				.WithDescription("process files line by line and print result (see also -n)")
				.Create('p'));
			options.AddOption(
				OptionBuilder.New().WithArgumentName("port")
				.hasOptionalArg()
				.WithDescription("listen on a port and process inbound lines")
				.Create('l'));
			options.AddOption(
				OptionBuilder.New().WithArgumentName("splitPattern")
				.hasOptionalArg()
				.WithDescription("split lines using splitPattern (default '\\s') using implicit 'split' variable")
				.WithLongName("autosplit")
				.Create('a'));

			Parser parser = new PosixParser(options);
			CommandLine line = parser.Parse(new String[] { "-e", "println 'hello'" }, true);

			Assert.IsTrue(line.HasOption('e'));
			Assert.AreEqual("println 'hello'", line.GetOptionValue('e'));
		}

		[Test]
		public void Man() {
			String cmdLine =
					"man [-c|-f|-k|-w|-tZT device] [-adlhu7V] [-Mpath] [-Ppager] [-Slist] " +
							"[-msystem] [-pstring] [-Llocale] [-eextension] [section] page ...";
			Options options = new Options().
					AddOption("a", "all", false, "find all matching manual pages.").
					AddOption("d", "debug", false, "emit debugging messages.").
					AddOption("e", "extension", false, "limit search to extension type 'extension'.").
					AddOption("f", "whatis", false, "equivalent to whatis.").
					AddOption("k", "apropos", false, "equivalent to apropos.").
					AddOption("w", "location", false, "print physical location of man page(s).").
					AddOption("l", "local-file", false, "interpret 'page' argument(s) as local filename(s)").
					AddOption("u", "update", false, "force a cache consistency check.").
				//FIXME - should generate -r,--prompt string
					AddOption("r", "prompt", true, "provide 'less' pager with prompt.").
					AddOption("c", "catman", false, "used by catman to reformat out of date cat pages.").
					AddOption("7", "ascii", false, "display ASCII translation or certain latin1 chars.").
					AddOption("t", "troff", false, "use troff format pages.").
				//FIXME - should generate -T,--troff-device device
					AddOption("T", "troff-device", true, "use groff with selected device.").
					AddOption("Z", "ditroff", false, "use groff with selected device.").
					AddOption("D", "default", false, "reset all options to their default values.").
				//FIXME - should generate -M,--manpath path
					AddOption("M", "manpath", true, "set search path for manual pages to 'path'.").
				//FIXME - should generate -P,--pager pager
					AddOption("P", "pager", true, "use program 'pager' to display output.").
				//FIXME - should generate -S,--sections list
					AddOption("S", "sections", true, "use colon separated section list.").
				//FIXME - should generate -m,--systems system
					AddOption("m", "systems", true, "search for man pages from other unix system(s).").
				//FIXME - should generate -L,--locale locale
					AddOption("L", "locale", true, "define the locale for this particular man search.").
				//FIXME - should generate -p,--preprocessor string
					AddOption("p", "preprocessor", true, "string indicates which preprocessor to run.\n" +
							 " e - [n]eqn  p - pic     t - tbl\n" +
							 " g - grap    r - refer   v - vgrind").
					AddOption("V", "version", false, "show version.").
					AddOption("h", "help", false, "show this usage message.");

			HelpFormatter hf = new HelpFormatter();
			hf.Options = options;
			//hf.printHelp(cmdLine, opts);
			hf.CommandLineSyntax = cmdLine;
			hf.PrintHelp();
		}

	}
}