using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sia.Domain.Playbook;
using Sia.Playbook.Requests;
using Sia.Shared.Authentication;
using Sia.Shared.Controllers;
using System.Threading.Tasks;

namespace Sia.Playbook.Controllers
{
    [Route("/eventTypes/")]
    public class EventTypeController : BaseController
    {
        public EventTypeController(IMediator mediator, AzureActiveDirectoryAuthenticationInfo authConfig, IUrlHelper urlHelper)
            : base(mediator, authConfig, urlHelper)
        {
        }

        [HttpGet("{id}", Name = nameof(Get) + nameof(EventType))]
        public async Task<IActionResult> Get(long id)
            => Ok(await _mediator.Send(new GetEventTypeRequest(id, _authContext)));
    }
}
