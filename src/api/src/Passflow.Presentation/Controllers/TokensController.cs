using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Passflow.Contracts.Dtos.Token;
using Passflow.Contracts.Exceptions;
using Passflow.Domain;
using Passflow.Infrastructure.Database;
using Swashbuckle.AspNetCore.Annotations;

namespace Passflow.Presentation.Controllers;
[Authorize]
public class TokensController : BaseApiController
{
    private readonly PassflowDbContext _context;

    public TokensController(PassflowDbContext context)
    {
        _context = context;
    }
    [SwaggerResponse(
	    StatusCodes.Status200OK,
	    "Token successfully created"
    )]
	[HttpPost("create")]
    public async Task<IActionResult> CreateTokenForUserAsync(TokenCreateDto tokenDto)
    {
        var user = await _context.Users.SingleOrDefaultAsync(e => e.UserName == GetAuthUserName());

        if (user == null)
            throw new UserNotFoundException($"User with name {GetAuthUserName()} not found!");

        var token = tokenDto.Adapt<Token>();
        token.UserId = user.Id;
        await _context.Tokens.AddAsync(token);

        return Ok();
    }
    [SwaggerResponse(
	    StatusCodes.Status200OK,
	    "Token successfully created",
        typeof(List<TokenDto>)
    )]

	[HttpGet]
    public async Task<IActionResult> GetAllUserTokensAsync()
    {
        var user = await _context.Users.SingleOrDefaultAsync(e => e.UserName == User.Identity!.Name);

        if (user == null)
            throw new UserNotFoundException($"User with name {GetAuthUserName()} not found!");

        var tokens = (await _context.Tokens.Where(e => e.UserId == user.Id).ToListAsync()).Adapt<List<TokenDto>>();

        return Ok(tokens);
    }
    [SwaggerResponse(
	    StatusCodes.Status200OK,
	    "Token successfully created",
	    typeof(TokenDto)
    )]
	[HttpGet("{tokenName}")]
    public async Task<IActionResult> GetTokenByNameAsync(string tokenName)
    {
        var user = await _context.Users.SingleOrDefaultAsync(e => e.UserName == GetAuthUserName());

        if (user is null)
            throw new UserNotFoundException($"User with name {GetAuthUserName()} not found!");

        var token = 
            _context.Tokens.SingleOrDefaultAsync(e => e.Name == tokenName 
            && e.UserId == user.Id);

        if (token is null)
            throw new TokenNotFoundException($"Token with name {tokenName} for user {GetAuthUserName()} not found!");

        var resultToken = token.Adapt<TokenDto>();

        return Ok(resultToken);
    }
}