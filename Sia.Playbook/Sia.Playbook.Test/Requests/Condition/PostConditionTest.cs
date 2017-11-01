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

namespace Sia.Playbook.Test.Requests.Condition
{
    [TestClass]
    public class PostConditionTest
    {
        [TestInitialize]
        public void ConfigureAutomapper() => AutoMapperStartup.InitializeAutomapper();

        [TestMethod]
        public async Task PostConditionHandler_Handle_WhenAddingNewRecord_ReturnCorrectRecord()
        {
            var context = await MockFactory.PlaybookContext(nameof(PostConditionHandler_Handle_WhenAddingNewRecord_ReturnCorrectRecord));

            var ConditionToAdd = new CreateCondition()
            {
                Name = "Newly Created Condition"
            };
            var conditionSetToAddTo = await context.ConditionSets.SingleAsync(at => at.Name == "Orphaned Condition Set");

            var serviceUnderTest = new PostConditionHandler(context);
            var request = new PostConditionRequest(conditionSetToAddTo.Id, ConditionToAdd, null);


            var result = await serviceUnderTest.Handle(request);


            Assert.AreEqual(ConditionToAdd.Name, result.Name);
        }

        [TestMethod]
        public async Task PostConditionHandler_Handle_WhenAddingNewRecord_RecordPersistsInContext()
        {
            var context = await MockFactory.PlaybookContext(nameof(PostConditionHandler_Handle_WhenAddingNewRecord_RecordPersistsInContext));

            var ConditionToAdd = new CreateCondition()
            {
                Name = "Newly Created Condition"
            };
            var conditionSetToAddTo = await context.ConditionSets.SingleAsync(at => at.Name == "Orphaned Condition Set");

            var serviceUnderTest = new PostConditionHandler(context);
            var request = new PostConditionRequest(conditionSetToAddTo.Id, ConditionToAdd, null);


            await serviceUnderTest.Handle(request);
            var result = await context.Conditions.SingleAsync(act => act.Name == "Newly Created Condition");


            Assert.AreEqual(ConditionToAdd.Name, result.Name);
            Assert.AreEqual(conditionSetToAddTo.Id, result.ConditionSetId);
        }
    }
}