using System;
using System.IO;
using System.Text;
using NDesk.Options;
using Tfs2Gource.Extensions;

namespace Tfs2Gource
{
	internal class Program
	{
		private static void Main(string[] args)
		{
            // Show help?
            bool showHelp = false;

            var gourceOptions = new Configuration.GourceOptions();

            var options = new OptionSet {
                { "t|tfs=", "The URL to access TFS. Example: scheme://domain:port/tfs/collection. (Required)", v => gourceOptions.TfsUrl = v },
                { "u|username=", "The TFS Username to connect to TFS.", v => gourceOptions.Username = v },
                { "p|password=", "The TFS Password to connect to TFS.", v => gourceOptions.UserPassword = v.GetBytes() },
                { "do|domain=", "The username's domain to connect to TFS.", v => gourceOptions.UserDomain = v },
                { "r|projectpath=", @"The TFS source location (as displayed in Visual Studio 'Source Control Explorer'). Example: $/teamProject/project/dev. Supports ';' seperated projects. Example: $/teamProject/projectA/dev;$/teamProject/projectB/dev. (Required)", v => gourceOptions.ProjectPath = v },
                { "m|minutes=", "Before now, how many minutes ago of changesets to return.", (int v) => gourceOptions.TimeSpan = TimeSpan.FromMinutes(v) },
                { "d|days=", "Before now, how many days ago of changesets to return. Default: 7 (overrides minutes).", (int v) => gourceOptions.TimeSpan = TimeSpan.FromDays(v) },
                { "o|output=", "The filename or filepath to save the log file to. Default: gource.log.", v => gourceOptions.OutputPath = v },
                { "h|help",  "Show this message and exit.", v => showHelp = v != null },
            };
            options.Parse(args);

            if (showHelp) {
                ShowHelp(options);
                return;
            }

            if (!gourceOptions.TfsUrl.HasValue()) {
                ShowHelp(options, "-t or -tfs= is a required field, please supply the URL to connect to TFS.");
                return;
            }
            
            if (!gourceOptions.ProjectPath.HasValue()) {
                ShowHelp(options, "-r or -projectpath= is a required field, please supply the TFS source path.");
                return;
            }

            // Configure some defaults is none are supplied.
            if (gourceOptions.TimeSpan.TotalMinutes == 0)
                gourceOptions.TimeSpan = TimeSpan.FromDays(7);

            if (!gourceOptions.OutputPath.HasValue())
                gourceOptions.OutputPath = "gource.log";

			FileInfo logFile;
			bool success = GenerateLogFile(gourceOptions, out logFile);
            if (success) {
                Console.WriteLine("Wrote out Gource log file to {0}", logFile.FullName);    
            }
		}

        private static void ShowHelp(OptionSet options, string errorMessage = "") {
            Console.WriteLine();
            if (errorMessage.HasValue()) {
                Console.WriteLine(" Error: {0}", errorMessage);
            }
            Console.WriteLine(" Tfs2Gource help:");
            Console.WriteLine();
            options.WriteOptionDescriptions(Console.Out);
        }

		private static bool GenerateLogFile(Configuration.GourceOptions gourceOptions, out FileInfo logFile)
		{
            
            logFile = new FileInfo(gourceOptions.OutputPath);
            if (File.Exists(logFile.FullName))
            {
				File.Delete(logFile.FullName);
            }

		    using (var fileStream = new FileStream(logFile.FullName, FileMode.Create, FileAccess.ReadWrite))
			{
				DateTime to = DateTime.Now.Date.AddDays(1);
				DateTime from = to.Subtract(gourceOptions.TimeSpan);
			    Converter.Process(gourceOptions, from, fileStream);
			}

			return (File.Exists(logFile.FullName) && logFile.Length > 0);
		}
	}
}