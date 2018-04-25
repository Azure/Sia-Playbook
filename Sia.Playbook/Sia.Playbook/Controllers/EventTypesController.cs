using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sia.Domain.Playbook;
using Sia.Playbook.Requests;
using Sia.Core.Authentication;
using Sia.Core.Controllers;
using System.Threading.Tasks;

namespace Sia.Playbook.Controllers
{
    [Route("/eventTypes/")]
    public class EventTypesController : BaseController
    {
        public EventTypesController(IMediator mediator, AzureActiveDirectoryAuthenticationInfo authConfig, IUrlHelper urlHelper)
            : base(mediator, authConfig, urlHelper)
        {
        }
        [HttpGet(Name = nameof(GetAll) + nameof(EventType))]
        public async Task<IActionResult> GetAll()
            => OkIfFound(await _mediator
                .Send(new GetEventTypesRequest(AuthContext))
                .ConfigureAwait(continueOnCapturedContext: false)
            );

        [HttpGet("{id}", Name = nameof(Get) + nameof(EventType))]
        public async Task<IActionResult> Get(long id)
            => OkIfFound(await _mediator
                .Send(new GetEventTypeRequest(id, AuthContext))
                .ConfigureAwait(continueOnCapturedContext: false)
            );
    }
}
