using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sia.Domain.Playbook;
using Sia.Core.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sia.Core.Configuration.Sources.GitHub;

namespace Sia.Playbook.Initialization
{
    public class PlaybookData
    {
        private const string ApplicationName = "Sia-Playbook";

        public IReadOnlyDictionary<long, EventType> EventTypes
            => _eventTypes;

        public IReadOnlyDictionary<long, Domain.Playbook.Action> GlobalActions
            => _globalActions;

        private Dictionary<long, EventType> _eventTypes = new Dictionary<long, EventType>();
        private Dictionary<long, Domain.Playbook.Action> _globalActions = new Dictionary<long, Domain.Playbook.Action>();

        public void RegisterData(IServiceCollection services)
            => services
                .AddSingleton(this)
                .AddSingleton(EventTypes)
                .AddSingleton(GlobalActions);

        public async Task LoadFromGithub(GitHubSourceConfiguration config, ILoggerFactory loggerFactory)
        {
            ThrowIf.Null(config, nameof(config));
            ThrowIf.Null(loggerFactory, nameof(loggerFactory));

            var client = config.GetClient(ApplicationName);

            var logger = loggerFactory.CreateLogger(nameof(PlaybookData));

            _eventTypes.AddSeedDataToDictionary(
                (await client.GetSeedDataFromGitHub<EventType>(
                    logger,
                    config.Repository,
                    nameof(EventType)
                ).ConfigureAwait(continueOnCapturedContext: false))
                .Select(tuple => tuple.resultObject)
            );

            _globalActions.AddSeedDataToDictionary(
                (await client.GetSeedDataFromGitHub<Domain.Playbook.Action>(
                    logger,
                    config.Repository,
                    "Global" + nameof(Domain.Playbook.Action)
                ).ConfigureAwait(continueOnCapturedContext: false))
                .Select(tuple => tuple.resultObject)
            );
        }
    }
}
