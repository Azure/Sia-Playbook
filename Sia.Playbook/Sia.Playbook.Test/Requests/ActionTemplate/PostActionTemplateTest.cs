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

namespace Sia.Playbook.Test.Requests.ActionTemplate
{
    [TestClass]
    public class PostActionTemplateTest
    {
        [TestInitialize]
        public void ConfigureAutomapper() => AutoMapperStartup.InitializeAutomapper();

        [TestMethod]
        public async Task PostActionTemplateHandler_Handle_WhenAddingNewRecord_ReturnCorrectRecord()
        {
            var context = await MockFactory.PlaybookContext(nameof(PostActionTemplateHandler_Handle_WhenAddingNewRecord_ReturnCorrectRecord));

            var ActionTemplateToAdd = new CreateActionTemplate()
            {
                Name = "Newly Created ActionTemplate"
            };

            var serviceUnderTest = new PostActionTemplateHandler(context);
            var request = new PostActionTemplateRequest(ActionTemplateToAdd, null);


            var result = await serviceUnderTest.Handle(request);


            Assert.AreEqual(ActionTemplateToAdd.Name, result.Name);
        }

        [TestMethod]
        public async Task PostActionTemplateHandler_Handle_WhenAddingNewRecord_RecordPersistsInContext()
        {
            var context = await MockFactory.PlaybookContext(nameof(PostActionTemplateHandler_Handle_WhenAddingNewRecord_RecordPersistsInContext));

            var ActionTemplateToAdd = new CreateActionTemplate()
            {
                Name = "Newly Created ActionTemplate"
            };

            var serviceUnderTest = new PostActionTemplateHandler(context);
            var request = new PostActionTemplateRequest(ActionTemplateToAdd, null);


            await serviceUnderTest.Handle(request);
            var result = await context.ActionTemplates.SingleAsync(act => act.Name == "Newly Created ActionTemplate");


            Assert.AreEqual(ActionTemplateToAdd.Name, result.Name);
        }
    }
}