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

namespace Sia.Playbook.Test.Requests.ActionTemplateSource
{
    [TestClass]
    public class PostActionTemplateSourceTest
    {
        [TestInitialize]
        public void ConfigureAutomapper() => AutoMapperStartup.InitializeAutomapper();

        [TestMethod]
        public async Task PostActionTemplateSourceHandler_Handle_WhenAddingNewRecord_ReturnCorrectRecord()
        {
            var context = await MockFactory.PlaybookContext(nameof(PostActionTemplateSourceHandler_Handle_WhenAddingNewRecord_ReturnCorrectRecord));

            var actionTemplateSourceToAdd = new CreateActionTemplateSource()
            {
                Name = "Newly Created ActionTemplateSource"
            };
            var actionTemplateToAddTo = await context.ActionTemplates.SingleAsync(at => at.Name == "Orphaned Action Template");

            var serviceUnderTest = new PostActionTemplateSourceHandler(context);
            var request = new PostActionTemplateSourceRequest(actionTemplateToAddTo.Id, actionTemplateSourceToAdd, null);


            var result = await serviceUnderTest.Handle(request);


            Assert.AreEqual(actionTemplateSourceToAdd.Name, result.Name);
        }

        [TestMethod]
        public async Task PostActionTemplateSourceHandler_Handle_WhenAddingNewRecord_RecordPersistsInContext()
        {
            var context = await MockFactory.PlaybookContext(nameof(PostActionTemplateSourceHandler_Handle_WhenAddingNewRecord_RecordPersistsInContext));

            var actionTemplateSourceToAdd = new CreateActionTemplateSource()
            {
                Name = "Newly Created ActionTemplateSource"
            };
            var actionTemplateToAddTo = await context.ActionTemplates.SingleAsync(at => at.Name == "Orphaned Action Template");

            var serviceUnderTest = new PostActionTemplateSourceHandler(context);
            var request = new PostActionTemplateSourceRequest(actionTemplateToAddTo.Id, actionTemplateSourceToAdd, null);


            await serviceUnderTest.Handle(request);
            var result = await context.ActionTemplateSources.SingleAsync(act => act.Name == "Newly Created ActionTemplateSource");


            Assert.AreEqual(actionTemplateSourceToAdd.Name, result.Name);
            Assert.AreEqual(actionTemplateToAddTo.Id, result.ActionTemplateId);
        }
    }
}