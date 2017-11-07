using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sia.Domain.ApiModels.Playbooks;
using Sia.Playbook.Initialization;
using Sia.Playbook.Requests;
using Sia.Playbook.Test.TestDoubles;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sia.Playbook.Test.Requests
{
    [TestClass]
    public class PostEventTypeTest
    {
        [TestInitialize]
        public void ConfigureAutomapper()
            => AutoMapperStartup.InitializeAutomapper();

        [TestMethod]
        public async Task PostEventTypeHandler_Handle_WhenAddingNewRecord_ReturnCorrectRecord()
        {
            var context = await MockFactory.PlaybookContext(nameof(PostEventTypeHandler_Handle_WhenAddingNewRecord_ReturnCorrectRecord));

            var eventTypeToAdd = new CreateEventType()
            {
                Name = "Newly Created EventType"
            };

            var serviceUnderTest = new PostEventTypeHandler(context);
            var request = new PostEventTypeRequest(eventTypeToAdd, null);


            var result = await serviceUnderTest.Handle(request);


            Assert.AreEqual(eventTypeToAdd.Name, result.Name);
        }

        [TestMethod]
        public async Task PostEventTypeHandler_Handle_WhenAddingNewRecord_RecordPersistsInContext()
        {
            var context = await MockFactory.PlaybookContext(nameof(PostEventTypeHandler_Handle_WhenAddingNewRecord_RecordPersistsInContext));

            var eventTypeToAdd = new CreateEventType()
            {
                Name = "Newly Created EventType"
            };

            var serviceUnderTest = new PostEventTypeHandler(context);
            var request = new PostEventTypeRequest(eventTypeToAdd, null);


            await serviceUnderTest.Handle(request);
            var result = await context.EventTypes.SingleAsync(act => act.Name == "Newly Created EventType");


            Assert.AreEqual(eventTypeToAdd.Name, result.Name);
        }
    }
}
