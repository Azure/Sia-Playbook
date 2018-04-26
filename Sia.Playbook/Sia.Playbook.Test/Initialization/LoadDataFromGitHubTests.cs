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
            var ExpectedOwner = "ExpectedOwner";
            var ExpectedRepositoryName = "ExpectedRepositoryName";

            var mockLogger = new StubLogger();
            var expectedSearchCodeRequest = new SearchCodeRequest("EventType", ExpectedOwner, ExpectedRepositoryName)
            {
                In = new[] { CodeInQualifier.Path },
                Extension = "json"
            };
            var expectedPathToEventTypeDictionary = new Dictionary<string, EventType>
            {
                { "firstExpectedPath", new EventType() { Id = 11, Name = "firstExpectedEventType" } },
                { "secondExpectedPath", new EventType() { Id = 12, Name = "secondExpectedEventType" } }
            };
            var mockConfig = MockGithubConfigFactory.Create(
                expectedSearchCodeRequest,
                expectedPathToEventTypeDictionary,
                1,
                ExpectedRepositoryName,
                ExpectedOwner
            );


            var resultObject = new Dictionary<long, EventType>();
            await resultObject
                .AddSeedDataFromGitHub(mockLogger, mockConfig, "EventType")
                .ConfigureAwait(continueOnCapturedContext: false);


            Assert.IsTrue(resultObject.TryGetValue(11, out var eventTypeFromLoadedDictionary));
            Assert.AreEqual("firstExpectedEventType", eventTypeFromLoadedDictionary.Name);
        }
    }
}
