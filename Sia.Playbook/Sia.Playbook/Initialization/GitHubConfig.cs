using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Octokit;
using Sia.Shared.Validation;
using Sia.Shared.Authentication;

namespace Sia.Playbook.Initialization
{
    public class GitHubConfig
    {
        public const string TokenConfigKey = "GitHub:Token";
        public const string RepositoryNameConfigKey = "GitHub:Repository:Name";
        public const string RepositoryOwnerConfigKey = "GitHub:Repository:Owner";

        public const string Application = "Sia-Playbook";

        private const string RepositoryLoadErrorMessage = "Failure to retrieve Github repository {0} with owner {1}";

        const string ConfigurationClientIdKey = "ClientId";
        const string ConfigurationClientSecretKey = "ClientSecret";
        const string ConfigurationVaultNameKey = "VaultName";
        const string ConfigurationGithubTokenNameKey = "GithubTokenName";
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
            var section = config.GetSection(TokenConfigKey);
            _token = section.Value;
            if (String.IsNullOrEmpty(_token))
            {
                var clientId = ThrowIf.NullOrWhiteSpace(config[ConfigurationClientIdKey], nameof(ConfigurationClientIdKey));
                var clientSecret = ThrowIf.NullOrWhiteSpace(config[ConfigurationClientSecretKey], nameof(ConfigurationClientSecretKey));
                var vaultName = ThrowIf.NullOrWhiteSpace(config[ConfigurationVaultNameKey], nameof(ConfigurationVaultNameKey));
                var tokenName = ThrowIf.NullOrWhiteSpace(config[ConfigurationGithubTokenNameKey], nameof(ConfigurationGithubTokenNameKey));

                var keyVaultConfig = new KeyVaultConfiguration(clientId, clientSecret, vaultName);
                var keyVault = new AzureSecretVault(keyVaultConfig);
                _token = keyVault.Get(tokenName).Result;
            }

            ThrowIf.NullOrWhiteSpace(_token, TokenConfigKey);
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
