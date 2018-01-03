using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Octokit;
using Sia.Shared.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sia.Playbook.Initialization
{
    public class GitHubConfig
    {
        public const string TokenConfigKey = "GitHub:Token";
        public const string RepositoryNameConfigKey = "GitHub:Repository:Name";
        public const string RepositoryOwnerConfigKey = "GitHub:Repository:Owner";

        public const string Application = "Sia-Playbook";

        private const string RepositoryLoadErrorMessage = "Failure to retrieve Github repository {0} with owner {1}";


        private readonly string _token;

        public virtual string RepositoryName { get; }
        public virtual string RepositoryOwner { get; }
        public virtual IGitHubClient Client {
            get
            {
                if(_client is null)
                {
                    _client = new GitHubClient(new ProductHeaderValue(Application))
                    {
                        Credentials = new Credentials(_token)
                    };
                }
                return _client;
            }
        }
        public virtual Repository Repository
        {
            get
            {
                if (_repository is null)
                {
                    var repositoryLoadTask = GetRepositoryAsync();
                    repositoryLoadTask.Wait();
                    _repository = repositoryLoadTask.Result;
                }
                return _repository;
            }
        }
        private readonly ILogger<GitHubConfig> _logger;
        private Repository _repository;
        private IGitHubClient _client;
        public GitHubConfig(IConfigurationRoot config, ILoggerFactory loggerFactory)
        {
            _token = ThrowIf.NullOrWhiteSpace(config[TokenConfigKey], nameof(TokenConfigKey));
            RepositoryName = ThrowIf.NullOrWhiteSpace(config[RepositoryNameConfigKey], nameof(RepositoryNameConfigKey));
            RepositoryOwner = ThrowIf.NullOrWhiteSpace(config[RepositoryOwnerConfigKey], nameof(RepositoryOwnerConfigKey));
            _logger = loggerFactory.CreateLogger<GitHubConfig>();
        }

        protected GitHubConfig()
        {
            //This constructor exists for testing purposes only
        }

        private async Task<Repository> GetRepositoryAsync()
        {
            try
            {
                return await Client.Repository.Get(RepositoryOwner, RepositoryName);
            }
            catch (Exception ex)
            {
                var errorMessageTokens = new object[] { RepositoryName, RepositoryOwner };
                _logger.LogError(
                    ex,
                    RepositoryLoadErrorMessage,
                    errorMessageTokens
                );
                throw new GitHubRepositoryRetrievalException(
                    String.Format(
                        RepositoryLoadErrorMessage, 
                        errorMessageTokens), 
                    ex);
            }
        }

    }
}
