using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sia.Playbook.Authentication;
using Sia.Domain.ApiModels.Playbooks;
using Sia.Data.Playbooks;
using Sia.Domain.Playbook;
using Sia.Domain;
using AutoMapper;

namespace Sia.Playbook.Requests
{
    public class PostActionRequest : AuthenticatedRequest<Domain.Playbook.Action>
    {
        public PostActionRequest(CreateAction createAction, AuthenticatedUserContext userContext) : base(userContext)
        {
            CreateAction = createAction;
        }

        public CreateAction CreateAction { get; }
    }

    public class PostActionHandler : DatabaseOperationHandler<PostActionRequest, Domain.Playbook.Action>
    {
        public PostActionHandler(PlaybookContext context) : base(context)
        {
        }

        public override async Task<Domain.Playbook.Action> Handle(PostActionRequest message)
        {
            var dataAction = Mapper.Map<Data.Playbooks.Models.Action>(message.CreateAction);

            var result = _context.Actions.Add(dataAction);
            await _context.SaveChangesAsync();

            return Mapper.Map<Domain.Playbook.Action>(dataAction);
        }
    }
}
