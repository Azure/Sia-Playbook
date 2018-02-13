using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sia.Domain.Playbook;
using Sia.Playbook.Initialization;
using Sia.Playbook.Requests;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Sia.Shared.Exceptions;

namespace Sia.Playbook.Test.Requests
{
    [TestClass]
    public class GetEventTypesTest
    {
        [TestMethod]
        public async Task GetEventTypesHandler_Handle_ReturnAllEventTypes()
        {
            var eventTypeIndex = new Dictionary<long, EventType>();

            var eventTypeToFind = new EventType()
            {
                Id = 5,
                Name = "Test Event Type"
            };
            var additionalEventType = new EventType()
            {
                Id = 4,
                Name = "UnusedEventType"
            };
            if (!eventTypeIndex.TryAdd(eventTypeToFind.Id, eventTypeToFind)) throw new Exception("Test setup failure when populating dictionary");
            if (!eventTypeIndex.TryAdd(additionalEventType.Id, additionalEventType)) throw new Exception("Test setup failure when populating dictionary");

            var serviceUnderTest = new GetEventTypesHandler(eventTypeIndex);

            var request = new GetEventTypesRequest(null);
            var result = await serviceUnderTest.Handle(request, cancellationToken: new CancellationToken());

            Assert.AreEqual(eventTypeIndex.Values.ElementAt(0), result.ElementAt(0));
        }

        [TestMethod]
        public async Task GetEventTypesHandler_Handle_WhenNoEventTypes_Return_Empty_Enumerable()
        {
            var eventTypeIndex = new Dictionary<long, EventType>();
            var serviceUnderTest = new GetEventTypesHandler(eventTypeIndex);
            var request = new GetEventTypesRequest(null);

            var result = await serviceUnderTest.Handle(request, cancellationToken: new CancellationToken());

            Assert.AreEqual(0, result.Count());
        }

    }
}
