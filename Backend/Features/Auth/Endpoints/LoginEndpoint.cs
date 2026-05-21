using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using TaskApi.Core;
using TaskApi.Core.Models;
using TaskApi.Features.Auth.Domain;

namespace TaskApi.Features.Auth.Endpoints;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Username).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}

public record LoginRequest(string Username, string Password);

public record LoginResponse(
    string AccessToken,
    string RefreshToken,
    int UserId,
    string Username,
    string Fullname
);

public class LoginEndpoint(
    UserManager<User> userManager,
    IJwtTokenService jwtTokenService
) : Endpoint<LoginRequest, ApiResponse<LoginResponse>>
{
    public override void Configure()
    {
        Post("/auth/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        var user = await userManager.FindByNameAsync(req.Username);
        
        if (user == null || !await userManager.CheckPasswordAsync(user, req.Password))
        {
            await Send.ResultAsync(TypedResults.BadRequest<ApiResponse>((Result)new Error("Auth.InvalidCredentials", "Invalid username or password")));
            return;
        }

        var roles = await userManager.GetRolesAsync(user);
        var (accessToken, refreshToken) = await jwtTokenService.GenerateTokensAsync(user, roles);

        await Send.OkAsync(Result.Success(new LoginResponse(
            accessToken,
            refreshToken,
            user.Id,
            user.UserName!,
            user.Fullname
        )), cancellation: ct);
    }
}
