using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Passflow.Application.Services;
using Passflow.Contracts;
using Passflow.Contracts.Dtos.Auth;
using Passflow.Infrastructure.Database;
using Passflow.Infrastructure.Services;

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
