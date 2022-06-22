using MarketGossip.ChatApp.Application.Features.Authentication.Models;
using MarketGossip.Shared.Dtos.Result;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace MarketGossip.ChatApp.Application.Features.Authentication.Commands;

public record RegisterUser(RegisterModel Payload) : IRequest<Result>;

public class RegisterUserHandler : IRequestHandler<RegisterUser, Result>
{
    private readonly UserManager<IdentityUser> _userManager;

    public RegisterUserHandler(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result> Handle(RegisterUser request, CancellationToken cancellationToken)
    {
        var model = request.Payload;

        var identityUsers = await Task.WhenAll(_userManager.FindByNameAsync(model.Username),
            _userManager.FindByEmailAsync(model.Email));

        if (identityUsers.Any(x => x is not null))
            return Result.Failure("Username or Email already in use!");

        var user = new IdentityUser
        {
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.Username
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded) return Result.Success();

        var errors = result.Errors.Select(e => e.Description).Aggregate((err, errB) => err + ", " + errB);

        return Result.Failure(errors);
    }
}