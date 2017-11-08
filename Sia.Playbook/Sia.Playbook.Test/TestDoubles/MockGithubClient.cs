using Moq;
using Newtonsoft.Json;
using Octokit;
using Sia.Domain.Playbook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sia.Playbook.Test.TestDoubles
{
    public class MockGithubClientFactory
    {
        public const string ExpectedOwner = nameof(ExpectedOwner);
        public const string ExpectedRepositoryName = nameof(ExpectedRepositoryName);
        public const long ExpectedRepositoryId = 1;
        public static IGitHubClient Create(SearchCodeRequest expectedRequest, Dictionary<string, EventType> pathsToEventTypes)
        {
            var client = new Mock<IGitHubClient>();
            client
                .Setup(mockClient => mockClient.Repository)
                .Returns(CreateRepositoriesClient(pathsToEventTypes));
            client
                .Setup(mockClient => mockClient.Search)
                .Returns(CreateSearchClient(expectedRequest, pathsToEventTypes.Keys.ToList()));
            return client.Object;
        }

        public static Task<Repository> CreateRepository()
        {
            /*var repositiory = new Mock<Repository>();
            repositiory
                .Setup(mockRepository => mockRepository.Id)
                .Returns(ExpectedRepositoryId);
            return Task.FromResult(repositiory.Object);
            */
            return Task.FromResult(new Repository(ExpectedRepositoryId));
        }

        public static IRepositoriesClient CreateRepositoriesClient(Dictionary<string, EventType> pathsToEventTypes)
        {
            var repositoriesClient = new Mock<IRepositoriesClient>();
            repositoriesClient
                .Setup(client => client.Get(ExpectedOwner, ExpectedRepositoryName))
                .Returns(CreateRepository());
            repositoriesClient
                .Setup(client => client.Content)
                .Returns(CreateRepositoryContentsClient(pathsToEventTypes));
            return repositoriesClient.Object;
        }

        private static IRepositoryContentsClient CreateRepositoryContentsClient(Dictionary<string, EventType> pathsToEventTypes)
        {
            var client = new Mock<IRepositoryContentsClient>();
            foreach (var pathEventTypePair in pathsToEventTypes)
            {
                client
                    .Setup(cl => cl.GetAllContents(ExpectedRepositoryId, pathEventTypePair.Key))
                    .Returns(Task.FromResult(GetRepositoryContent(pathEventTypePair.Value)));
            }
            return client.Object;
        }

        private static IReadOnlyList<RepositoryContent> GetRepositoryContent(EventType eventType)
        {
            var content = new RepositoryContent(
                null,
                null,
                null,
                0,
                ContentType.File,
                null,
                null,
                null,
                null,
                null,
                Convert.ToBase64String( Encoding.UTF8.GetBytes( JsonConvert.SerializeObject(eventType))),
                null,
                null
                );
            return new List<RepositoryContent>() { content };
        }

        public static ISearchClient CreateSearchClient(SearchCodeRequest expectedRequest, List<string> resultPaths)
        {
            var searchClient = new Mock<ISearchClient>();
            searchClient
                .Setup(client => client.SearchCode(It.IsAny<SearchCodeRequest>()))
                .Returns(Task.FromResult(CreateSearchCodeResult(resultPaths)));
            return searchClient.Object;
        }

        public static SearchCodeResult CreateSearchCodeResult(List<string> pathsToWrap)
        {
            var result = new SearchCodeResult(2, false, CreateSearchCodeList(pathsToWrap));
            return result;
        }

        public static IReadOnlyList<SearchCode> CreateSearchCodeList(List<string> pathsToWrap)
            => pathsToWrap.Select(path => GetSearchCode(path)).ToList();
        

        public static SearchCode GetSearchCode(string path)
        {
            var code = new SearchCode(null, path, null, null, null, null, null);
            return code;
        }
    }
}
