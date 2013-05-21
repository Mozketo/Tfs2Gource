using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tfs2Gource.Extensions;

namespace Tfs2Gource.Configuration {
    public class GourceOptions {
        // TFS URL like scheme://url:port/teamCollection
        public string TfsUrl { get; set; }
        
        // Username like domain\username
        public string Username { get; set; }

        // Password
        public byte[] UserPassword { get; set; }
        
        // Domain
        public string UserDomain { get; set; }
        
        // The TFS project path like $\teamProject\project
        public string ProjectPath { get; set; }
        
        // How long ago to fetch data
        public TimeSpan TimeSpan { get; set; }

        // Output like file.log or c:\path\file.log
        public string OutputPath { get; set; }
    }
}
