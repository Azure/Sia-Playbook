using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sia.Playbook.Initialization;
using Sia.Playbook.Requests;
using Sia.Playbook.Test.TestDoubles;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sia.Playbook.Test.Requests.ConditionSet
{
    [TestClass]
    public class GetConditionSetTest
    {
        [TestInitialize]
        public void ConfigureAutomapper() => AutoMapperStartup.InitializeAutomapper();

        [TestMethod]
        public async Task GetConditionSetHandler_Handle_WhenRecordExists_ReturnCorrectRecord()
        {
            var context = await MockFactory.PlaybookContext(nameof(GetConditionSetHandler_Handle_WhenRecordExists_ReturnCorrectRecord));

            var conditionSetToFind = await context.ConditionSets.SingleAsync(ats => ats.Name == "Condition Set For Orphaned Action");
            var parentOfConditionSetToFind = await context.Actions.SingleAsync(at => at.Name == "Orphaned Action");

            var serviceUnderTest = new GetConditionSetHandler(context);
            var request = new GetConditionSetRequest(conditionSetToFind.Id, parentOfConditionSetToFind.Id, null);


            var result = await serviceUnderTest.Handle(request);


            Assert.AreEqual(conditionSetToFind.Name, result.Name);
        }
    }
}
