using System.Threading.Tasks;
using Sia.Data.Playbooks;
using Sia.Shared.Authentication;
using Microsoft.EntityFrameworkCore;
using Sia.Shared.Requests;

namespace Sia.Playbook.Requests
{
    public class AssociateActionWithEventTypeRequest : AuthenticatedRequest
    {
        public AssociateActionWithEventTypeRequest(long actionId, long eventTypeId, AuthenticatedUserContext userContext)
            : base(userContext)
        {
            ActionId = actionId;
            EventTypeId = eventTypeId;
        }

        public long ActionId { get; }
        public long EventTypeId { get; }
    }

    public class AssociateActionWithEventTypeHandler
        : PlaybookDatabaseHandler<AssociateActionWithEventTypeRequest>
    {
        public AssociateActionWithEventTypeHandler(PlaybookContext context) : base(context)
        {
        }

        public override async Task Handle(AssociateActionWithEventTypeRequest message)
        {
            var dataEventType = _context.EventTypes.FirstAsync(record => record.Id == message.EventTypeId);
            var dataAction = _context.Actions.FirstAsync(record => record.Id == message.ActionId);
            Task.WaitAll(dataEventType, dataAction);

            dataEventType.Result.Actions.Add(dataAction.Result);

            await _context.SaveChangesAsync();
        }
    }
}
