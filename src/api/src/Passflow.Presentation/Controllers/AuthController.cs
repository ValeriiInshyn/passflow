using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Passflow.Application.Services;
using Passflow.Contracts;
using Passflow.Contracts.Dtos.Auth;
using Passflow.Infrastructure.Database;
using Passflow.Infrastructure.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace Passflow.Presentation.Controllers;

public class AuthController : BaseApiController
{
	private readonly PassflowDbContext _context;
	private readonly ITokenService _tokenService;

	public AuthController(PassflowDbContext context, ITokenService tokenService)
	{
		_context = context;
		_tokenService = tokenService;
	}
	[SwaggerOperation(
		Summary = "Login",
		Description = "ApiLogin"
	)]
	[SwaggerResponse(
		StatusCodes.Status200OK,
		"User successfully updated",
		typeof(TokenResponse)
	)]
	[SwaggerResponse(
		StatusCodes.Status400BadRequest,
		"User successfully updated",
		typeof(ValidationProblemDetails)
	)]
	[HttpPost("login")]
	public async Task<IActionResult> LoginAsync(LoginDto request)
	{
		var foundUser = await _context.Users.SingleOrDefaultAsync(u => u.UserName == request.UserName);
		if (foundUser is null || !PasswordEncrypter.VerifyPassword(request.Password, foundUser.PasswordHash))
			return ValidationProblem("Email or Password is incorrect");

		return Ok(new TokenResponse
		{
			AccessToken = _tokenService.GenerateAccessToken(request.UserName, foundUser.IsAdmin ? Roles.Admin : Roles.User),
			RefreshToken = _tokenService.GenerateRefreshToken(request.UserName),
		});
	}
}
