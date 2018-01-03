using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sia.Playbook.Initialization
{
    public class GitHubRepositoryRetrievalException : Exception
    {
        public GitHubRepositoryRetrievalException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
