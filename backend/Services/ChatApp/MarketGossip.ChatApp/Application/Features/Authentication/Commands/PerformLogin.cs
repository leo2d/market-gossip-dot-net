using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MarketGossip.ChatApp.Application.Features.Authentication.Models;
using MarketGossip.Shared.Dtos.Result;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace MarketGossip.ChatApp.Application.Features.Authentication.Commands;

public record PerformLogin(LoginModel Payload) : IRequest<Result<LoginResponse>>;

public record LoginResponse(string Token, DateTime Expiration);

public class LoginHandler : IRequestHandler<PerformLogin, Result<LoginResponse>>
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<IdentityUser> _userManager;

    public LoginHandler(UserManager<IdentityUser> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<Result<LoginResponse>> Handle(PerformLogin request, CancellationToken cancellationToken)
    {
        var model = request.Payload;

        var user = await _userManager.FindByNameAsync(model.Username);
        var passwordIsCorrect = await _userManager.CheckPasswordAsync(user, model.Password);

        if (user is null) return Result.Failure<LoginResponse>("User does not exist");
        if (!passwordIsCorrect) return Result.Failure<LoginResponse>("Invalid password");

        var userRoles = await _userManager.GetRolesAsync(user);

        var authClaims = new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        authClaims.AddRange(userRoles.Select(userRole => new Claim(ClaimTypes.Role, userRole)));

        var token = GetToken(authClaims);

        var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);
        return Result.Success(new LoginResponse(tokenStr, token.ValidTo));
    }

    private JwtSecurityToken GetToken(IEnumerable<Claim> authClaims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:ValidIssuer"],
            audience: _configuration["JWT:ValidAudience"],
            expires: DateTime.Now.AddHours(3),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return token;
    }
}