using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Tfs2Gource.Extensions;

namespace Tfs2Gource {
    public static class Converter {
        /// <summary>
        /// {timestamp}|{user}|{action}|{filepath}|";
        /// </summary>
        public const string LogFormat = "{0}|{1}|{2}|{3}|";

        private static readonly IDictionary<ChangeType, string> AllowedTypes = new Dictionary<ChangeType, string> {
            {ChangeType.Add, "A"},
            {ChangeType.Edit, "M"},
            {ChangeType.Delete, "D"},
        };

        public static void Process(Configuration.GourceOptions gourceOptions, DateTime from, Stream outStream) {
            Process(gourceOptions.TfsUrl, gourceOptions.Username, 
                gourceOptions.UserPassword, gourceOptions.UserDomain, 
                gourceOptions.ProjectPath, from, outStream);
        }

		public static void Process(string tfsUrl, string username, byte[] userPassword, string userDomain, string projectPath, DateTime from, Stream outStream)
		{
            var projectPaths = projectPath.Split(Convert.ToChar(";"));
            var credential = new System.Net.NetworkCredential(username, userPassword.AsString(), userDomain);
            var server = new TfsTeamProjectCollection(new Uri(tfsUrl), credential);
			server.Authenticate();

            // Get the Changeset list from the TFS API.
			var history = new List<Changeset>();
			foreach (var path in projectPaths)
			{
                var source = server.GetService<VersionControlServer>();
                var projectPathTemp = path.Trim();
                Trace.WriteLine(String.Format("Searching history for project {0}", projectPathTemp));

				var projectHistory = source.QueryHistory(projectPathTemp, VersionSpec.Latest, 0, RecursionType.Full,
														  null, new DateVersionSpec(from), null, int.MaxValue,
														  true,
														  false, false, false).OfType<Changeset>().Reverse().ToList();
				projectHistory = projectHistory.Where(item => item.CreationDate > from).ToList();
				history.AddRange(projectHistory);
			}
			var orderedHistory = history.OrderBy(m => m.CreationDate);

            // Using the list of changesets transform the output to match what Gource requires.
			using (var writer = new StreamWriter(outStream))
			{
				foreach (var item in orderedHistory)
				{
					Trace.WriteLine(String.Format("Found changeset id = {0}. Committed: {1}", item.ChangesetId, item.CreationDate));

					foreach (Change change in item.Changes)
					{
						ChangeType changeType = change.ChangeType;
						if (!AllowedTypes.Any(type => (type.Key & changeType) != 0))
							continue;

						KeyValuePair<ChangeType, string> code = AllowedTypes.FirstOrDefault(type => (type.Key & changeType) != 0);
						writer.WriteLine(LogFormat, item.CreationDate.ToUnix(), item.Committer, code.Value, change.Item.ServerItem);
					}
				}
			}
			Console.WriteLine("Processing finished");
		}
    }
}