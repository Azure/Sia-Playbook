using Newtonsoft.Json;
using Octokit;
using Sia.Domain.Playbook;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sia.Playbook.Initialization
{
    public static class LoadDataFromGitHub
    {
        public static GitHubClient GetAuthenticatedClient(string gitHubToken, string application = "Sia-Playbook")
        {
            var tokenAuth = new Credentials(gitHubToken);
            var client = new GitHubClient(new ProductHeaderValue(application))
            {
                Credentials = tokenAuth
            };
            return client;
        }

        public static async Task AddSeedDataFromGitHub(this ConcurrentDictionary<long, EventType> eventTypeIndex, IGitHubClient client, string repositoryName, string repositoryOwner)
        {
            var repo = await client.Repository.Get(repositoryOwner, repositoryName);

            var request = new SearchCodeRequest("EventType", repositoryOwner, repositoryName)
            {
                In = new[] { CodeInQualifier.Path },
                Extension = "json"
            };

            var result = await client.Search.SearchCode(request);

            var eventTypesToAddTasks = result
                .Items
                .Select(async item
                    => (await client
                    .Repository
                    .Content
                    .GetAllContents(repo.Id, item.Path))
                ).ToArray();

            Task.WaitAll(eventTypesToAddTasks);
            var eventTypesToAdd = eventTypesToAddTasks
                .Select(task 
                    => task.IsCompletedSuccessfully 
                    ? task.Result
                    : null);

            var content = eventTypesToAdd
                .SelectMany(et => et
                    .Select(rc 
                        => JsonConvert.DeserializeObject<EventType>(rc.Content)
                    )
                );

            eventTypeIndex.AddSeedDataToDictionary(content);
        }

        public static void AddSeedDataToDictionary(this ConcurrentDictionary<long, EventType> eventTypeIndex, IEnumerable<EventType> toAdd)
        {
            foreach (var item in toAdd)
            {
                eventTypeIndex.TryAdd(item.Id, item);
            }
        }
    }
}
