using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sia.Playbook.Authentication;
using Sia.Domain.ApiModels.Playbooks;
using Sia.Data.Playbooks;
using Sia.Domain.Playbook;
using Sia.Domain;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Sia.Playbook.Requests
{
    public class PostConditionSetRequest : AuthenticatedRequest<Domain.Playbook.ConditionSet>
    {
        public PostConditionSetRequest(long actionId, CreateConditionSet createConditionSet, AuthenticatedUserContext userContext) : base(userContext)
        {
            CreateConditionSet = createConditionSet;
            ActionId = actionId;
        }

        public CreateConditionSet CreateConditionSet { get; }
        public long ActionId { get; }
    }

    public class PostConditionSetHandler : DatabaseOperationHandler<PostConditionSetRequest, Domain.Playbook.ConditionSet>
    {
        public PostConditionSetHandler(PlaybookContext context) : base(context)
        {
        }

        public override async Task<ConditionSet> Handle(PostConditionSetRequest message)
        {
            var dataConditionSet = Mapper.Map<Data.Playbooks.Models.ConditionSet>(message.CreateConditionSet);

            var dataAction = await _context.Actions.FirstAsync(at => at.Id == message.ActionId);
            dataAction.ConditionSets.Add(dataConditionSet);
            await _context.SaveChangesAsync();

            return Mapper.Map<ConditionSet>(dataConditionSet);
        }
    }
}
