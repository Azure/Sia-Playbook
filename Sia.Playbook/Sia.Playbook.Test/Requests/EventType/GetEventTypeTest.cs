using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sia.Data.Playbooks;
using Sia.Domain.Playbook;
using Sia.Playbook.Initialization;
using Sia.Playbook.Requests;
using Sia.Playbook.Test.TestDoubles;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Sia.Playbook.Test.Requests
{
    [TestClass]
    public class GetEventTypeTest
    {
        [TestInitialize]
        public void ConfigureAutomapper()
            => AutoMapperStartup.InitializeAutomapper();

        [TestMethod]
        public async Task GetEventTypeHandler_Handle_WhenRecordExists_ReturnCorrectRecord()
        {
            var context = await MockFactory.PlaybookContext(nameof(GetEventTypeHandler_Handle_WhenRecordExists_ReturnCorrectRecord));
            var eventTypeIndex = new ConcurrentDictionary<long, EventType>();
            eventTypeIndex.AddSeedDataToDictionary(await context.EventTypes.WithEagerLoading().ProjectTo<EventType>().ToListAsync());

            var eventTypeToFind = await context.EventTypes.SingleAsync(act => act.Name == "Orphaned Event Type");



            var serviceUnderTest = new GetEventTypeHandler(eventTypeIndex);
            var request = new GetEventTypeRequest(eventTypeToFind.Id, null);


            var result = await serviceUnderTest.Handle(request);


            Assert.AreEqual(eventTypeToFind.Name, result.Name);
        }
    }
}
