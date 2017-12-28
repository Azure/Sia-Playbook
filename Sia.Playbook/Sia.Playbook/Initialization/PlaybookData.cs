using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sia.Domain.Playbook;
using Sia.Shared.Validation;
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

        public async Task LoadFromGithub(ILoggerFactory loggerFactory, string token, string name, string owner)
        {
            ThrowIf.NullOrWhiteSpace(token, nameof(token));
            ThrowIf.NullOrWhiteSpace(name, nameof(name));
            ThrowIf.NullOrWhiteSpace(owner, nameof(owner));

            var logger = loggerFactory.CreateLogger(nameof(PlaybookData));
                
            var client = LoadDataFromGitHub.GetAuthenticatedClient(token);
            var repo = await LoadDataFromGitHub.GetRepositoryAsync(logger, client, name, owner);

            await _eventTypes.AddSeedDataFromGitHub(
                repo,
                logger,
                client,
                name,
                owner,
                nameof(EventType));

            await _globalActions.AddSeedDataFromGitHub(
                repo,
                logger,
                client,
                name,
                owner,
                "Global" + nameof(Domain.Playbook.Action));
        }
    }
}
