using Moq;
using Newtonsoft.Json;
using Octokit;
using Sia.Domain.Playbook;
using Sia.Playbook.Initialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sia.Playbook.Test.TestDoubles
{
    public class MockGithubConfigFactory
    {

        public static GitHubConfig Create<T>(
            SearchCodeRequest expectedRequest,
            Dictionary<string, T> pathsToRecords,
            long expectedRepositoryId,
            string repoName, 
            string repoOwner)
        {
            var config = new Mock<MockableGitHubConfig>();
            config
                .Setup(mockConfig => mockConfig.RepositoryName)
                .Returns(repoName);
            config
                .Setup(mockConfig => mockConfig.RepositoryOwner)
                .Returns(repoOwner);
            config
                .Setup(mockConfig => mockConfig.Client)
                .Returns(
                    CreateClient(
                        expectedRequest,
                        pathsToRecords,
                        expectedRepositoryId,
                        repoName,
                        repoOwner
                    )
                );
            config
                .Setup(mockConfig => mockConfig.Repository)
                .Returns(CreateRepository(expectedRepositoryId));
            return config.Object;
        }

        public static IGitHubClient CreateClient<T>(
            SearchCodeRequest expectedRequest, 
            Dictionary<string, T> pathsToRecords,
            long expectedRepositoryId,
            string expectedRepositoryName,
            string expectedOwner
        )
        {
            var client = new Mock<IGitHubClient>();
            client
                .Setup(mockClient => mockClient.Repository)
                .Returns(
                    CreateRepositoriesClient(
                        pathsToRecords,
                        expectedRepositoryId,
                        expectedOwner,
                        expectedRepositoryName
                    )
                );
            client
                .Setup(mockClient => mockClient.Search)
                .Returns(CreateSearchClient(expectedRequest, pathsToRecords.Keys.ToList()));
            return client.Object;
        }

        public static Repository CreateRepository(long expectedRepositoryId)
            => new Repository(expectedRepositoryId);
        

        public static IRepositoriesClient CreateRepositoriesClient<T>(
            Dictionary<string, T> pathsToRecords,
            long expectedRepositoryId,
            string expectedOwner,
            string expectedRepositoryName
        )
        {
            var repositoriesClient = new Mock<IRepositoriesClient>();
            repositoriesClient
                .Setup(client => client.Get(expectedOwner, expectedRepositoryName))
                .Returns(Task.FromResult(CreateRepository(expectedRepositoryId)));
            repositoriesClient
                .Setup(client => client.Content)
                .Returns(CreateRepositoryContentsClient(pathsToRecords, expectedRepositoryId));
            return repositoriesClient.Object;
        }

        private static IRepositoryContentsClient CreateRepositoryContentsClient<T>(
            Dictionary<string, T> pathsToRecords,
            long expectedRepositoryId
        )
        {
            var client = new Mock<IRepositoryContentsClient>();
            foreach (var pathEventTypePair in pathsToRecords)
            {
                client
                    .Setup(cl => cl.GetAllContents(expectedRepositoryId, pathEventTypePair.Key))
                    .Returns(Task.FromResult(GetRepositoryContent(pathEventTypePair.Value)));
            }
            return client.Object;
        }

        private static IReadOnlyList<RepositoryContent> GetRepositoryContent<T>(
            T record
        )
        {
            //https://developer.github.com/v3/repos/contents/
            //The only part that's used by our test is the actual base64 contents
            //So most metadata arguments are faked (null)
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
                Convert.ToBase64String( Encoding.UTF8.GetBytes( JsonConvert.SerializeObject(record))),
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
            => new SearchCodeResult(2, false, CreateSearchCodeList(pathsToWrap));

        public static IReadOnlyList<SearchCode> CreateSearchCodeList(List<string> pathsToWrap)
            => pathsToWrap.Select(path => GetSearchCode(path)).ToList();
        
        public static SearchCode GetSearchCode(string path)
            => new SearchCode(null, path, null, null, null, null, null);
    }

    public class MockableGitHubConfig : GitHubConfig
    {

    }
}
