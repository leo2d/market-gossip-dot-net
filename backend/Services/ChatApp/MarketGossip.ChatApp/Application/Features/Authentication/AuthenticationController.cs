using MarketGossip.ChatApp.Application.Features.Authentication.Commands;
using MarketGossip.ChatApp.Application.Features.Authentication.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MarketGossip.ChatApp.Application.Features.Authentication;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthenticationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Route("signin")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var result = await _mediator.Send(new PerformLogin(model));

        return result.IsSuccess ? Ok(result.Value) : Unauthorized(result.Error);
    }

    [HttpPost]
    [Route("signup")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        var result = await _mediator.Send(new RegisterUser(model));

        if (!result.IsSuccess)
            return BadRequest(new {Status = "Failure", result.Error});

        return Ok(new {Status = "Success", Message = "User created successfully!"});
    }
}