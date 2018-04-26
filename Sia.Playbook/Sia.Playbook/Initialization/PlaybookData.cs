using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sia.Domain.Playbook;
using Sia.Core.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sia.Playbook.Initialization
{
    public class PlaybookData
    {

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

        public async Task LoadFromGithub(GitHubConfig config, ILoggerFactory loggerFactory)
        {
            ThrowIf.Null(config, nameof(config));
            ThrowIf.Null(loggerFactory, nameof(loggerFactory));

            var logger = loggerFactory.CreateLogger(nameof(PlaybookData));
                
            await _eventTypes.AddSeedDataFromGitHub(
                logger,
                config,
                nameof(EventType)
            ).ConfigureAwait(continueOnCapturedContext: false);

            await _globalActions.AddSeedDataFromGitHub(
                logger,
                config,
                "Global" + nameof(Domain.Playbook.Action)
            ).ConfigureAwait(continueOnCapturedContext: false);
        }
    }
}
