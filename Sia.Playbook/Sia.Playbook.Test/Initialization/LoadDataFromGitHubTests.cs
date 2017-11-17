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

namespace Sia.Playbook.Test.Initialization
{
    [TestClass]
    public class LoadDataFromGitHubTests
    {
        [TestMethod]
        public async Task WhenEventTypesExist_ExpectedEventTypesAreReturned()
        {
            ILoggerFactory mockLogger = new StubLoggerFactory();
            var expectedSearchCodeRequest = new SearchCodeRequest("EventType", MockGithubClientFactory.ExpectedOwner, MockGithubClientFactory.ExpectedRepositoryName)
            {
                In = new[] { CodeInQualifier.Path },
                Extension = "json"
            };
            var expectedEventTypes = new Dictionary<string, EventType>
            {
                { "firstExpectedPath", new EventType() { Id = 11, Name = "firstExpectedEventType" } },
                { "secondExpectedPath", new EventType() { Id = 12, Name = "secondExpectedEventType" } }
            };
            var mockClient = MockGithubClientFactory.Create(expectedSearchCodeRequest, expectedEventTypes);
            var resultObject = new ConcurrentDictionary<long, EventType>();


            await resultObject.AddSeedDataFromGitHub(mockLogger, mockClient, MockGithubClientFactory.ExpectedRepositoryName, MockGithubClientFactory.ExpectedOwner);


            Assert.IsTrue(resultObject.TryGetValue(11, out var eventTypeFromLoadedDictionary));
            Assert.AreEqual("firstExpectedEventType", eventTypeFromLoadedDictionary.Name);
        }
    }
}
