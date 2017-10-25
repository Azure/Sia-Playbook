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
    public class PostConditionRequest : AuthenticatedRequest<Domain.Playbook.Condition>
    {
        public PostConditionRequest(long conditionSetId, CreateCondition createCondition, AuthenticatedUserContext userContext) : base(userContext)
        {
            CreateCondition = createCondition;
            ConditionSetId = conditionSetId;
        }

        public CreateCondition CreateCondition { get; }
        public long ConditionSetId { get; }
    }

    public class PostConditionHandler : DatabaseOperationHandler<PostConditionRequest, Domain.Playbook.Condition>
    {
        public PostConditionHandler(PlaybookContext context) : base(context)
        {
        }

        public override async Task<Condition> Handle(PostConditionRequest message)
        {
            var dataCondition = Mapper.Map<Data.Playbooks.Models.Condition>(message.CreateCondition);

            var dataConditionSet = await _context.ConditionSets.FirstAsync(at => at.Id == message.ConditionSetId);
            dataConditionSet.Conditions.Add(dataCondition);
            await _context.SaveChangesAsync();

            return Mapper.Map<Condition>(dataCondition);
        }
    }
}
