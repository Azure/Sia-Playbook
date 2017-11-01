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

namespace Sia.Playbook.Test.Requests.Action
{
    [TestClass]
    public class PostActionTest
    {
        [TestInitialize]
        public void ConfigureAutomapper() => AutoMapperStartup.InitializeAutomapper();

        [TestMethod]
        public async Task PostActionHandler_Handle_WhenAddingNewRecord_ReturnCorrectRecord()
        {
            var context = await MockFactory.PlaybookContext(nameof(PostActionHandler_Handle_WhenAddingNewRecord_ReturnCorrectRecord));

            var actionToAdd = new CreateAction()
            {
                Name = "Newly Created Action"
            };

            var serviceUnderTest = new PostActionHandler(context);
            var request = new PostActionRequest(actionToAdd, null);


            var result = await serviceUnderTest.Handle(request);


            Assert.AreEqual(actionToAdd.Name, result.Name);
        }

        [TestMethod]
        public async Task PostActionHandler_Handle_WhenAddingNewRecord_RecordPersistsInContext()
        {
            var context = await MockFactory.PlaybookContext(nameof(PostActionHandler_Handle_WhenAddingNewRecord_RecordPersistsInContext));

            var actionToAdd = new CreateAction()
            {
                Name = "Newly Created Action"
            };

            var serviceUnderTest = new PostActionHandler(context);
            var request = new PostActionRequest(actionToAdd, null);


            await serviceUnderTest.Handle(request);
            var result = await context.Actions.SingleAsync(act => act.Name == "Newly Created Action");


            Assert.AreEqual(actionToAdd.Name, result.Name);
        }
    }
}
