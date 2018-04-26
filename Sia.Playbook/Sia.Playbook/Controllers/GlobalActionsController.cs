using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sia.Domain.Playbook;
using Sia.Playbook.Requests;
using Sia.Core.Authentication;
using Sia.Core.Controllers;
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
            => OkIfFound(await _mediator
                .Send(new GetGlobalActionsRequest(AuthContext))
                .ConfigureAwait(continueOnCapturedContext: false)
            );
    }
}
