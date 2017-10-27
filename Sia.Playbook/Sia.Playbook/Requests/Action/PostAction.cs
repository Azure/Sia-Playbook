using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sia.Shared.Authentication;
using Sia.Domain.ApiModels.Playbooks;
using Sia.Data.Playbooks;
using Sia.Domain.Playbook;
using Sia.Domain;
using AutoMapper;
using Sia.Shared.Requests;

namespace Sia.Playbook.Requests
{
    public class PostActionRequest : AuthenticatedRequest<Action>
    {
        public PostActionRequest(CreateAction createAction, AuthenticatedUserContext userContext) : base(userContext)
        {
            CreateAction = createAction;
        }

        public CreateAction CreateAction { get; }
    }

    public class PostActionHandler : PlaybookDatabaseHandler<PostActionRequest, Action>
    {
        public PostActionHandler(PlaybookContext context) : base(context)
        {
        }

        public override async Task<Action> Handle(PostActionRequest message)
        {
            var dataAction = Mapper.Map<Data.Playbooks.Models.Action>(message.CreateAction);

            var result = _context.Actions.Add(dataAction);
            await _context.SaveChangesAsync();

            return Mapper.Map<Action>(dataAction);
        }
    }
}
