using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    public class GetConditionTest
    {
        [TestInitialize]
        public void ConfigureAutomapper() => AutoMapperStartup.InitializeAutomapper();

        [TestMethod]
        public async Task GetConditionHandler_Handle_WhenRecordExists_ReturnCorrectRecord()
        {
            var context = await MockFactory.PlaybookContext(nameof(GetConditionHandler_Handle_WhenRecordExists_ReturnCorrectRecord));

            var conditionToFind = await context.Conditions.SingleAsync(act => act.Name == "Condition for Orphaned Condition Set");
            var parentOfConditionToFind = await context.ConditionSets.SingleAsync(cs => cs.Name == "Orphaned Condition Set");

            var serviceUnderTest = new GetConditionHandler(context);
            var request = new GetConditionRequest(conditionToFind.Id, parentOfConditionToFind.Id, null);


            var result = await serviceUnderTest.Handle(request);


            Assert.AreEqual(conditionToFind.Name, result.Name);
        }
    }
}
