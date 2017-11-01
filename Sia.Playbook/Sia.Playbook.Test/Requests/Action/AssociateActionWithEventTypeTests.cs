using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sia.Playbook.Requests;
using Sia.Playbook.Test.TestDoubles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sia.Playbook.Test.Requests.Action
{
    [TestClass]
    public class AssociateActionWithEventTypeTests
    {
        [TestMethod]
        public async Task AssociateActionWithEventTypeHandler_Handle_WhenBothRecordsExist_CorrectlyAddsAssociationRecord()
        {
            var context = await MockFactory.PlaybookContext(nameof(AssociateActionWithEventTypeHandler_Handle_WhenBothRecordsExist_CorrectlyAddsAssociationRecord));

            var actionToAssociate = await context.Actions.SingleAsync(act => act.Name == "Orphaned Action");
            var eventTypeToAssociate = await context.EventTypes.SingleAsync(et => et.Name == "Orphaned Event Type");

            var serviceUnderTest = new AssociateActionWithEventTypeHandler(context);
            var request = new AssociateActionWithEventTypeRequest(actionToAssociate.Id, eventTypeToAssociate.Id, null);


            await serviceUnderTest.Handle(request);
            var result = context.EventTypeToActionAssociations.SingleAsync(ettaa => ettaa.ActionId == actionToAssociate.Id && ettaa.EventTypeId == eventTypeToAssociate.Id);


            //SingleAsync not throwing exception means this test passed
        }
    }
}
