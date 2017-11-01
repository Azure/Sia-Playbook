using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    public class GetActionTemplateTest
    {
        [TestInitialize]
        public void ConfigureAutomapper()
            => AutoMapperStartup.InitializeAutomapper();

        [TestMethod]
        public async Task GetActionTemplateHandler_Handle_WhenRecordExists_ReturnCorrectRecord()
        {
            var context = await MockFactory.PlaybookContext(nameof(GetActionTemplateHandler_Handle_WhenRecordExists_ReturnCorrectRecord));

            var actionTemplateToFind = await context.ActionTemplates.SingleAsync(act => act.Name == "Orphaned Action Template");

            var serviceUnderTest = new GetActionTemplateHandler(context);
            var request = new GetActionTemplateRequest(actionTemplateToFind.Id, null);


            var result = await serviceUnderTest.Handle(request);


            Assert.AreEqual(actionTemplateToFind.Name, result.Name);
        }
    }
}
