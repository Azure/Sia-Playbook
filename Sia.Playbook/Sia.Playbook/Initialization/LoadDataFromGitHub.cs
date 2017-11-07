using Newtonsoft.Json;
using Octokit;
using Sia.Data.Playbooks;
using Sia.Data.Playbooks.Models;
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

        public static async Task AddSeedDataFromGitHub(this PlaybookContext context, string gitHubToken, string repositoryName, string repositoryOwner)
        {
            var client = GetAuthenticatedClient(gitHubToken);

            var repo = await client.Repository.Get(repositoryOwner, repositoryName);

            var request = new SearchCodeRequest("EventType", repositoryOwner, repositoryName)
            {
                In = new[] { CodeInQualifier.Path },
                Extension = "json"
            };

            var result = await client.Search.SearchCode(request);

            var eventTypesDictionary = new ConcurrentDictionary<string, IReadOnlyList<RepositoryContent>>();
            var eventTypesToAddTasks = result
                .Items
                .Select(item
                    => client
                    .Repository
                    .Content
                    .GetAllContents(repo.Id, item.Path)
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

            context.EventTypes.AddRange(content.ToArray());
            await context.SaveChangesAsync();
        }
    }
}
