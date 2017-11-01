using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sia.Playbook.Initialization;
using Sia.Playbook.Requests;
using Sia.Playbook.Test.TestDoubles;
using System.Threading.Tasks;

namespace Sia.Playbook.Test.Requests.EventType
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

            var eventTypeToFind = await context.EventTypes.SingleAsync(act => act.Name == "Orphaned Event Type");

            var serviceUnderTest = new GetEventTypeHandler(context);
            var request = new GetEventTypeRequest(eventTypeToFind.Id, null);


            var result = await serviceUnderTest.Handle(request);


            Assert.AreEqual(eventTypeToFind.Name, result.Name);
        }
    }
}
