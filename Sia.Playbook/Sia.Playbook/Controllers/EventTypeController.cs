using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Sia.Playbook.Authentication;
using Sia.Playbook.Requests;

namespace Sia.Playbook.Controllers
{
    [Route("/eventTypes/")]
    public class EventTypeController : BaseController
    {
        public EventTypeController(IMediator mediator, AzureActiveDirectoryAuthenticationInfo authConfig, IUrlHelper urlHelper)
            : base(mediator, authConfig, urlHelper)
        {
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
            => Ok(await _mediator.Send(new GetEventTypeRequest(id, _authContext)));
    }
}
