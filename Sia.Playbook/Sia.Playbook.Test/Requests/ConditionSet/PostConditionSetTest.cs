using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sia.Domain.ApiModels.Playbooks;
using Sia.Playbook.Initialization;
using Sia.Playbook.Requests;
using Sia.Playbook.Test.TestDoubles;
using System.Threading.Tasks;

namespace Sia.Playbook.Test.Requests.ConditionSet
{
    [TestClass]
    public class PostConditionSetTest
    {
        [TestInitialize]
        public void ConfigureAutomapper() => AutoMapperStartup.InitializeAutomapper();

        [TestMethod]
        public async Task PostConditionSetHandler_Handle_WhenAddingNewRecord_ReturnCorrectRecord()
        {
            var context = await MockFactory.PlaybookContext(nameof(PostConditionSetHandler_Handle_WhenAddingNewRecord_ReturnCorrectRecord));

            var conditionSetToAdd = new CreateConditionSet()
            {
                Name = "Newly Created ConditionSet"
            };
            var actionToAddTo = await context.Actions.SingleAsync(at => at.Name == "Orphaned Action");

            var serviceUnderTest = new PostConditionSetHandler(context);
            var request = new PostConditionSetRequest(actionToAddTo.Id, conditionSetToAdd, null);


            var result = await serviceUnderTest.Handle(request);


            Assert.AreEqual(conditionSetToAdd.Name, result.Name);
        }

        [TestMethod]
        public async Task PostConditionSetHandler_Handle_WhenAddingNewRecord_RecordPersistsInContext()
        {
            var context = await MockFactory.PlaybookContext(nameof(PostConditionSetHandler_Handle_WhenAddingNewRecord_RecordPersistsInContext));

            var conditionSetToAdd = new CreateConditionSet()
            {
                Name = "Newly Created ConditionSet"
            };
            var actionToAddTo = await context.Actions.SingleAsync(at => at.Name == "Orphaned Action");

            var serviceUnderTest = new PostConditionSetHandler(context);
            var request = new PostConditionSetRequest(actionToAddTo.Id, conditionSetToAdd, null);


            await serviceUnderTest.Handle(request);
            var result = await context.ConditionSets.SingleAsync(act => act.Name == "Newly Created ConditionSet");


            Assert.AreEqual(conditionSetToAdd.Name, result.Name);
            Assert.AreEqual(actionToAddTo.Id, result.ActionId);
        }
    }
}