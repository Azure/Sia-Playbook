using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sia.Playbook.Initialization;
using Sia.Playbook.Requests;
using Sia.Playbook.Test.TestDoubles;
using System.Threading.Tasks;

namespace Sia.Playbook.Test.Requests.ConditionSource
{
    [TestClass]
    public class GetConditionSourceTest
    {
        [TestInitialize]
        public void ConfigureAutomapper() => AutoMapperStartup.InitializeAutomapper();

        [TestMethod]
        public async Task GetConditionSourceHandler_Handle_WhenRecordExists_ReturnCorrectRecord()
        {
            var context = await MockFactory.PlaybookContext(nameof(GetConditionSourceHandler_Handle_WhenRecordExists_ReturnCorrectRecord));

            var conditionSourceToFind = await context.ConditionSources.SingleAsync(act => act.Name == "Orphaned Condition Source");

            var serviceUnderTest = new GetConditionSourceHandler(context);
            var request = new GetConditionSourceRequest(conditionSourceToFind.Id, null);


            var result = await serviceUnderTest.Handle(request);


            Assert.AreEqual(conditionSourceToFind.Name, result.Name);
        }
    }
}
