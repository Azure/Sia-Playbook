using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    public class GetActionTemplateSourceTest
    {
        [TestInitialize]
        public void ConfigureAutomapper() => AutoMapperStartup.InitializeAutomapper();

        [TestMethod]
        public async Task GetActionTemplateSourceHandler_Handle_WhenRecordExists_ReturnCorrectRecord()
        {
            var context = await MockFactory.PlaybookContext(nameof(GetActionTemplateSourceHandler_Handle_WhenRecordExists_ReturnCorrectRecord));

            var actionTemplateSourceToFind = await context.ActionTemplateSources.SingleAsync(ats => ats.Name == "Source for Orphaned Action Template");
            var parentOfActionTemplateSourceToFind = await context.ActionTemplates.SingleAsync(at => at.Name == "Orphaned Action Template");

            var serviceUnderTest = new GetActionTemplateSourceHandler(context);
            var request = new GetActionTemplateSourceRequest(actionTemplateSourceToFind.Id, parentOfActionTemplateSourceToFind.Id, null);


            var result = await serviceUnderTest.Handle(request);


            Assert.AreEqual(actionTemplateSourceToFind.Name, result.Name);
        }
    }
}
