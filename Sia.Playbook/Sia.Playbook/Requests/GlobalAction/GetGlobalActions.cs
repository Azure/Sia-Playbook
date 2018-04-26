using MediatR;
using Sia.Domain.Playbook;
using Sia.Core.Authentication;
using Sia.Core.Requests;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sia.Playbook.Requests
{
    public class GetGlobalActionsRequest : AuthenticatedRequest<IEnumerable<Action>>
    {
        public GetGlobalActionsRequest(AuthenticatedUserContext userContext)
            : base(userContext)
        {
        }
    }

    public class GetGlobalActionsHandler : AsyncRequestHandler<GetGlobalActionsRequest, IEnumerable<Action>>
    {
        private readonly IReadOnlyDictionary<long, Action> _index;

        public GetGlobalActionsHandler(IReadOnlyDictionary<long, Action> GlobalActionIndex)
        {
            _index = GlobalActionIndex;
        }

        protected override Task<IEnumerable<Action>> HandleCore(GetGlobalActionsRequest message)
            => Task.FromResult(_index.Values);
    }
}