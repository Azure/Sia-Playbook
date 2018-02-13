using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sia.Domain.Playbook;
using Sia.Playbook.Requests;
using Sia.Shared.Authentication;
using Sia.Shared.Controllers;
using System.Threading.Tasks;

namespace Sia.Playbook.Controllers
{
    [Route("/globalActions/")]
    public class GlobalActionsController : BaseController
    {
        public GlobalActionsController(IMediator mediator, AzureActiveDirectoryAuthenticationInfo authConfig, IUrlHelper urlHelper)
            : base(mediator, authConfig, urlHelper)
        {
        }
        [HttpGet(Name = nameof(GetAll) + "Global" + nameof(Action))]
        public async Task<IActionResult> GetAll()
            => OkIfAny(await _mediator.Send(new GetGlobalActionsRequest(_authContext)));
    }
}
