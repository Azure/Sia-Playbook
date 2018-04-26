using Sia.Core.Authentication;
using Sia.Core.Configuration.Sources.GitHub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sia.Playbook.Configuration
{
    public class PlaybookConfig : KeyVaultConfiguration
    {
        const string ApplicationName = "Sia-Playbook";
        public string GithubTokenName { get; set; }
        public GitHubSourceConfiguration GitHub { get; set; }
    }
}
