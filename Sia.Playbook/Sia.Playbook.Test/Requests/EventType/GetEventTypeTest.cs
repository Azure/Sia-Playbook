﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sia.Domain.Playbook;
using Sia.Playbook.Initialization;
using Sia.Playbook.Requests;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Sia.Core.Exceptions;

namespace Sia.Playbook.Test.Requests
{
    [TestClass]
    public class GetEventTypeTest
    {
        [TestMethod]
        public async Task GetEventTypeHandler_Handle_WhenRecordExists_ReturnCorrectRecord()
        {
            var eventTypeIndex = new ConcurrentDictionary<long, EventType>();

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

            var serviceUnderTest = new GetEventTypeHandler(eventTypeIndex);
            var request = new GetEventTypeRequest(eventTypeToFind.Id, null);

            var result = await serviceUnderTest
                .Handle(request, cancellationToken: new CancellationToken())
                .ConfigureAwait(continueOnCapturedContext: false);

            Assert.AreEqual(eventTypeToFind.Name, result.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(NotFoundException))]
        public async Task GetEventTypeHandler_Handle_When_RecordDoesNotExist_Throw_NotFoundException()
        {
            var eventTypeIndex = new ConcurrentDictionary<long, EventType>();
            var serviceUnderTest = new GetEventTypeHandler(eventTypeIndex);
            var missingId = 1000;
            var request = new GetEventTypeRequest(missingId, null);

            var result = await serviceUnderTest
                .Handle(request, cancellationToken: new CancellationToken())
                .ConfigureAwait(continueOnCapturedContext: false);

            // Expect exception
        }
    }
}
