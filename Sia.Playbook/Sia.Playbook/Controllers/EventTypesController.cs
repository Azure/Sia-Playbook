using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sia.Domain.Playbook;
using Sia.Playbook.Requests;
using Sia.Shared.Authentication;
using Sia.Shared.Controllers;
using System.Collections.Generic;
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
        {
            var result = await _mediator.Send(new GetEventTypesRequest(_authContext));
            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("{id}", Name = nameof(Get) + nameof(EventType))]
        public async Task<IActionResult> Get(long id)
        {
            try
            {
                var result = await _mediator.Send(new GetEventTypeRequest(id, _authContext));
                if (result != null)
                {
                    return Ok(result);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound();
            }

        }
    }
}
