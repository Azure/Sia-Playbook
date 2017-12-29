using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sia.Domain.Playbook;
using Sia.Playbook.Initialization;
using Sia.Playbook.Requests;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Sia.Playbook.Test.Requests.GlobalAction
{
    [TestClass]
    public class GetGlobalActionsTest
    {
        [TestMethod]
        public async Task GetGlobalActionsHandler_Handle_ReturnAllGlobalActions()
        {
            var globalActionIndex = new Dictionary<long, Domain.Playbook.Action>();

            var globalActionToFind = new Domain.Playbook.Action()
            {
                Id = 5,
                Name = "Test Action"
            };
            var additionalAction = new Domain.Playbook.Action()
            {
                Id = 4,
                Name = "UnusedAction"
            };
            if (!globalActionIndex.TryAdd(globalActionToFind.Id, globalActionToFind)) throw new Exception("Test setup failure when populating dictionary");
            if (!globalActionIndex.TryAdd(additionalAction.Id, additionalAction)) throw new Exception("Test setup failure when populating dictionary");

            var serviceUnderTest = new GetGlobalActionsHandler(globalActionIndex);

            var request = new GetGlobalActionsRequest(null);
            var result = await serviceUnderTest.Handle(request);

            Assert.AreEqual(globalActionIndex.Values.ElementAt(0), result.ElementAt(0));
        }
    }
}
