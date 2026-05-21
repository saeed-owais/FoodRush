using FoodRush.API.Extensions;
using FoodRush.Application.Features.Authentication.Register;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FoodRush.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterCommand request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return result.IsSuccess
            ? StatusCode(StatusCodes.Status201Created, result.Value)
            : result.Problem();
        }
    }
}
