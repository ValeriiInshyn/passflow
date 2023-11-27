using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Passflow.Contracts.Dtos.Token;
using Passflow.Contracts.Exceptions;
using Passflow.Domain;
using Passflow.Infrastructure.Database;
using Swashbuckle.AspNetCore.Annotations;

namespace Passflow.Presentation.Controllers;

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
    public async Task<IActionResult> CreateTokenForUser(TokenCreateDto tokenDto, string username)
    {
        var user = await _context.Users.SingleOrDefaultAsync(e => e.UserName == username);

        if (user == null)
            throw new UserNotFoundException($"User with name {username} not found!");

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
    public async Task<IActionResult> GetAllTokensForAuthenticateUser()
    {
        var user = await _context.Users.SingleOrDefaultAsync(e => e.UserName == "username");

        if (user == null)
            throw new UserNotFoundException($"User with name {"username"} not found!");

        var tokens = (await _context.Tokens.Where(e => e.UserId == user.Id).ToListAsync()).Adapt<List<TokenDto>>();

        return Ok(tokens);
    }
    [SwaggerResponse(
	    StatusCodes.Status200OK,
	    "Token successfully created",
	    typeof(TokenDto)
    )]
	[HttpGet("token-name={tokenName}")]
    public async Task<IActionResult> GetTokenByName(string tokenname, string username)
    {
        var user = await _context.Users.SingleOrDefaultAsync(e => e.UserName == username);

        if (user is null)
            throw new UserNotFoundException($"User with name {username} not found!");

        var token = 
            _context.Tokens.SingleOrDefaultAsync(e => e.Name == tokenname 
            && e.UserId == user.Id);

        if (token is null)
            throw new TokenNotFoundException($"Token with name {tokenname} for user {username} not found!");

        var resultToken = token.Adapt<TokenDto>();

        return Ok(resultToken);
    }
}