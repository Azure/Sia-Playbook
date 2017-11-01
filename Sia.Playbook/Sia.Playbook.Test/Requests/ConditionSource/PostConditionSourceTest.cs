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

namespace Sia.Playbook.Test.Requests.ConditionSource
{
    [TestClass]
    public class PostConditionSourceTest
    {
        [TestInitialize]
        public void ConfigureAutomapper() => AutoMapperStartup.InitializeAutomapper();

        [TestMethod]
        public async Task PostConditionSourceHandler_Handle_WhenAddingNewRecord_ReturnCorrectRecord()
        {
            var context = await MockFactory.PlaybookContext(nameof(PostConditionSourceHandler_Handle_WhenAddingNewRecord_ReturnCorrectRecord));

            var conditionSourceToAdd = new CreateConditionSource()
            {
                Name = "Newly Created ConditionSource"
            };

            var serviceUnderTest = new PostConditionSourceHandler(context);
            var request = new PostConditionSourceRequest(conditionSourceToAdd, null);


            var result = await serviceUnderTest.Handle(request);


            Assert.AreEqual(conditionSourceToAdd.Name, result.Name);
        }

        [TestMethod]
        public async Task PostConditionSourceHandler_Handle_WhenAddingNewRecord_RecordPersistsInContext()
        {
            var context = await MockFactory.PlaybookContext(nameof(PostConditionSourceHandler_Handle_WhenAddingNewRecord_RecordPersistsInContext));

            var conditionSourceToAdd = new CreateConditionSource()
            {
                Name = "Newly Created ConditionSource"
            };

            var serviceUnderTest = new PostConditionSourceHandler(context);
            var request = new PostConditionSourceRequest(conditionSourceToAdd, null);


            await serviceUnderTest.Handle(request);
            var result = await context.ConditionSources.SingleAsync(act => act.Name == "Newly Created ConditionSource");


            Assert.AreEqual(conditionSourceToAdd.Name, result.Name);
        }
    }
}