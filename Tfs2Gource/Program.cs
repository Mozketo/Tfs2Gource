using System;
using System.IO;
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
                { "t|tfs=", "The TFS URL like scheme://url:port/teamCollection.", v => gourceOptions.TfsUrl = v },
                { "u|username=", "Username to TFS.", v => gourceOptions.Username = v },
                { "p|password=", "Password to TFS.", v => gourceOptions.UserPassword = v.GetBytes() },
                { "do|domain=", "Domain of user.", v => gourceOptions.UserDomain = v },
                { "r|projectpath=", @"The TFS project path like $\teamProject\project.", v => gourceOptions.ProjectPath = v },
                { "m|minutes=", "How many minutes worth of changesets to fetch.", (int v) => gourceOptions.TimeSpan = TimeSpan.FromMinutes(v) },
                { "d|days=", "How many days worth of changesets to fetch (overrides).", (int v) => gourceOptions.TimeSpan = TimeSpan.FromDays(v) },
                { "o|output=", "Path to output.", v => gourceOptions.OutputPath = v },
                { "h|help",  "show this message and exit", v => showHelp = v != null },
            };
            options.Parse(args);

            if (showHelp || !gourceOptions.TfsUrl.HasValue() || !gourceOptions.ProjectPath.HasValue())
                ShowHelp();

			FileInfo logFile;
			bool success = GenerateLogFile(gourceOptions, out logFile);
            if (success) {
                Console.WriteLine("Gource log file written");
            }
		}

        private static void ShowHelp() {
            Console.WriteLine("Help here");
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