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
        public static async Task AddSeedDataFromGitHub<T>(
            this Dictionary<long, T> index,
            ILogger logger,
            GitHubConfig config,
            string searchTerm
        )
            where T: class, IEntity
        {
            var request = new SearchCodeRequest(searchTerm, config.RepositoryOwner, config.RepositoryName)
            {
                In = new[] { CodeInQualifier.Path },
                Extension = "json"
            };

            var result = await config.Client.Search.SearchCode(request);

            var recordsToAddTasks = result
                .Items
                .Select(ExtractContents(config.Client, config.Repository))
                .ToArray();

            Task.WaitAll(
                recordsToAddTasks
                    .Select(taskTuple => taskTuple.contentsTask)
                    .ToArray()
            );
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
