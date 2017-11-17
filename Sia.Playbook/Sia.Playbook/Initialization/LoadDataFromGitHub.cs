using Microsoft.Extensions.Logging;
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

        public static async Task AddSeedDataFromGitHub(
            this ConcurrentDictionary<long, EventType> eventTypeIndex,
            ILoggerFactory loggerFactory,
            IGitHubClient client,
            string repositoryName,
            string repositoryOwner
        )
        {
            var logger = loggerFactory.CreateLogger(nameof(LoadDataFromGitHub));

            Repository repo;
            try
            {
                repo = await client.Repository.Get(repositoryOwner, repositoryName);
            }
            catch (ApiException ex)
            {
                logger.LogError(
                    ex,
                    "Failure to retrieve Github repository {0} with owner {1}",
                    new object[] { repositoryName, repositoryOwner }
                );
                return;
            }

            var request = new SearchCodeRequest("EventType", repositoryOwner, repositoryName)
            {
                In = new[] { CodeInQualifier.Path },
                Extension = "json"
            };

            var result = await client.Search.SearchCode(request);

            var eventTypesToAddTasks = result
                .Items
                .Select(ExtractContents(client, repo)).ToArray();

            Task.WaitAll(eventTypesToAddTasks
                .Select(taskTuple => taskTuple.contentsTask)
                .ToArray());
            var eventTypesToAdd = eventTypesToAddTasks
                .Where(taskTuple => taskTuple.contentsTask.IsCompletedSuccessfully)
                .Select(taskTuple => (
                    contents: taskTuple.contentsTask.Result,
                    filePath: taskTuple.filePath)
                );

            LogFileRetrievalFailures(logger, eventTypesToAddTasks);

            var content = eventTypesToAdd
                .SelectMany(DeserializeContents(logger))
                .Where(et => !(et is null));

            eventTypeIndex.AddSeedDataToDictionary(content);
        }

        private static void LogFileRetrievalFailures(ILogger logger, (Task<IReadOnlyList<RepositoryContent>> contentsTask, string filePath)[] eventTypesToAddTasks)
        {
            foreach (var failedTask
                in eventTypesToAddTasks
                    .Where(taskTuple => !taskTuple.contentsTask.IsCompletedSuccessfully))
            {
                if (failedTask.contentsTask.Exception is null)
                {
                    logger.LogError(
                        "Failure when trying to read file contents from {0}",
                        new object[] { failedTask.filePath }
                    );
                }
                else
                {
                    logger.LogError(
                        failedTask.contentsTask.Exception,
                        "Failure when trying to read file contents from {0}",
                        new object[] { failedTask.filePath }
                    );
                }
            }
        }

        private static Func<(IReadOnlyList<RepositoryContent> contents, string filePath), IEnumerable<EventType>> DeserializeContents(ILogger logger)
        => ((IReadOnlyList<RepositoryContent> contents, string filePath) contentTuple)
        => contentTuple.contents.Select(TryDeserialize(logger, contentTuple.filePath));

        private static Func<RepositoryContent, EventType> TryDeserialize(ILogger logger, string filePath)
        {
            EventType tryDeserialize(RepositoryContent content)
            {
                try
                {
                    return JsonConvert.DeserializeObject<EventType>(content.Content);
                }
                catch (Exception ex)
                {
                    logger.LogError(
                        ex,
                        "Failure when trying to deserialize eventType json from file {0}",
                        new object[] { filePath }
                    );
                    return null;
                }
            }
            return tryDeserialize;
        }


        private static Func<SearchCode, (Task<IReadOnlyList<RepositoryContent>> contentsTask, string filePath)> ExtractContents(
            IGitHubClient client,
            Repository repo
        )
        => (SearchCode item) 
        => (contentsTask: client
                .Repository
                .Content
                .GetAllContents(repo.Id, item.Path),
            filePath: item.Path);

        public static void AddSeedDataToDictionary(this ConcurrentDictionary<long, EventType> eventTypeIndex, IEnumerable<EventType> toAdd)
        {
            foreach (var item in toAdd)
            {
                eventTypeIndex.TryAdd(item.Id, item);
            }
        }
    }
}
