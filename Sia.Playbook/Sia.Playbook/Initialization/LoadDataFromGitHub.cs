using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Octokit;
using Sia.Domain.Playbook;
using Sia.Shared.Data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sia.Playbook.Initialization
{
    public static class LoadDataFromGitHub
    {
        
        public static GitHubClient GetAuthenticatedClient(
            string gitHubToken, 
            string application = "Sia-Playbook")
        {
            var tokenAuth = new Credentials(gitHubToken);
            var client = new GitHubClient(new ProductHeaderValue(application))
            {
                Credentials = tokenAuth
            };
            return client;
        }

        public static async Task<Repository> GetRepositoryAsync(
            ILogger logger,
            IGitHubClient client,
            string repositoryName,
            string repositoryOwner
        )
        {
            try
            {
                return await client.Repository.Get(repositoryOwner, repositoryName);
            }
            catch (Exception ex)
            {
                var errorMessageTokens = new object[] { repositoryName, repositoryOwner };
                logger.LogError(
                    ex,
                    errorMessage,
                    errorMessageTokens
                );
                throw new GitHubRepositoryRetrievalException(String.Format(errorMessage, errorMessageTokens), ex);
            }
        }
        private const string errorMessage = "Failure to retrieve Github repository {0} with owner {1}";

        public static async Task AddSeedDataFromGitHub<T>(
            this Dictionary<long, T> index,
            Repository repo,
            ILogger logger,
            IGitHubClient client,
            string repositoryName,
            string repositoryOwner,
            string searchTerm
        )
            where T: class, IEntity
        {
            var request = new SearchCodeRequest(searchTerm, repositoryOwner, repositoryName)
            {
                In = new[] { CodeInQualifier.Path },
                Extension = "json"
            };

            var result = await client.Search.SearchCode(request);

            var recordsToAddTasks = result
                .Items
                .Select(ExtractContents(client, repo)).ToArray();

            Task.WaitAll(recordsToAddTasks
                .Select(taskTuple => taskTuple.contentsTask)
                .ToArray());
            var recordsToAdd = recordsToAddTasks
                .Where(taskTuple => taskTuple.contentsTask.IsCompletedSuccessfully)
                .Select(taskTuple => (
                    contents: taskTuple.contentsTask.Result,
                    filePath: taskTuple.filePath)
                );

            LogFileRetrievalFailures(logger, recordsToAddTasks);

            var content = recordsToAdd
                .SelectMany(DeserializeContents<T>(logger))
                .Where(et => !(et is null));

            index.AddSeedDataToDictionary(content);
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

        private static Func<(IReadOnlyList<RepositoryContent> contents, string filePath), IEnumerable<T>> DeserializeContents<T>(ILogger logger)
            where T: class
        => ((IReadOnlyList<RepositoryContent> contents, string filePath) contentTuple)
        => contentTuple.contents.Select(TryDeserialize<T>(logger, contentTuple.filePath));

        private static Func<RepositoryContent, T> TryDeserialize<T>(ILogger logger, string filePath)
            where T: class
        {
            T tryDeserialize(RepositoryContent content)
            {
                try
                {
                    return JsonConvert.DeserializeObject<T>(content.Content);
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

        public static void AddSeedDataToDictionary<T>(
            this Dictionary<long, T> index, 
            IEnumerable<T> toAdd)
            where T: IEntity
        {
            foreach (var item in toAdd)
            {
                index.Add(item.Id, item);
            }
        }
    }
}
