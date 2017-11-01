using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sia.Playbook.Initialization;
using Sia.Playbook.Requests;
using Sia.Playbook.Test.TestDoubles;
using System.Threading.Tasks;

namespace Sia.Playbook.Test.Requests.Action
{
    [TestClass]
    public class GetActionTest
    {
        [TestInitialize]
        public void ConfigureAutomapper() => AutoMapperStartup.InitializeAutomapper();

        [TestMethod]
        public async Task GetActionHandler_Handle_WhenRecordExists_ReturnCorrectRecord()
        {
            var context = await MockFactory.PlaybookContext(nameof(GetActionHandler_Handle_WhenRecordExists_ReturnCorrectRecord));

            var actionToFind = await context.Actions.SingleAsync(act => act.Name == "Orphaned Action");

            var serviceUnderTest = new GetActionHandler(context);
            var request = new GetActionRequest(actionToFind.Id, null);


            var result = await serviceUnderTest.Handle(request);


            Assert.AreEqual(actionToFind.Name, result.Name);
        }
    }
}
