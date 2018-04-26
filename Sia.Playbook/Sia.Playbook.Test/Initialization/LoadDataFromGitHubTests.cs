using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Octokit;
using Sia.Domain.Playbook;
using Sia.Playbook.Initialization;
using Sia.Playbook.Test.TestDoubles;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Sia.Core.Configuration.Sources.GitHub;
using System.Linq;

namespace Sia.Playbook.Test.Initialization
{
    [TestClass]
    public class LoadDataFromGitHubTests
    {
        [TestMethod]
        public async Task WhenEventTypesExist_ExpectedEventTypesAreReturned()
        {
            var repoConfig = new GitHubRepositoryConfiguration()
            {
                Owner = "ExpectedOwner",
                Name = "ExpectedRepositoryName"
            };

            var mockLogger = new StubLogger();
            var expectedSearchCodeRequest = new SearchCodeRequest("EventType", repoConfig.Owner, repoConfig.Name)
            {
                In = new[] { CodeInQualifier.Path },
                Extension = "json"
            };
            var expectedPathToEventTypeDictionary = new Dictionary<string, EventType>
            {
                { "firstExpectedPath", new EventType() { Id = 11, Name = "firstExpectedEventType" } },
                { "secondExpectedPath", new EventType() { Id = 12, Name = "secondExpectedEventType" } }
            };
            var client = GitHubMocksFactory.CreateClient<EventType>(
                expectedSearchCodeRequest,
                expectedPathToEventTypeDictionary,
                1,
                repoConfig
            );

            var result = await client
                .GetSeedDataFromGitHub<EventType>(mockLogger, repoConfig, "EventType")
                .ConfigureAwait(continueOnCapturedContext: false);
            var firstLoaded = result.First();


            Assert.AreEqual(11, firstLoaded.resultObject.Id);
            Assert.AreEqual("firstExpectedEventType", firstLoaded.resultObject.Name);
        }
    }
}
