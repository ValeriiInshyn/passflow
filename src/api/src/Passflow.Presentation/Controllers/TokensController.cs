using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Passflow.Contracts.Dtos;
using Passflow.Contracts.Exceptions;
using Passflow.Domain;
using Passflow.Infrastructure.Database;

namespace Passflow.Presentation.Controllers;

public class TokensController : Controller
{
    private readonly PassflowDbContext _context;

    public TokensController(PassflowDbContext context)
    {
        _context = context;
    }

    [HttpPost("tokens/create")]
    public async Task<IActionResult> CreateTokenForUser(TokenDto tokenDto, string username)
    {
        var user = await _context.Users.SingleAsync(e => e.UserName == username);

        if (user == null)
            throw new UserNotFoundException();

        var token = tokenDto.Adapt<Token>();
        token.UserId = user.Id;
        await _context.Tokens.AddAsync(token);

        return Ok(tokenDto);
    }


    [HttpGet("tokens/username={username}")]
    public async Task<IActionResult> GetAllTokensForUser(string username)
    {
        var user = await _context.Users.SingleAsync(e => e.UserName == username);

        if (user == null)
            throw new UserNotFoundException();

        var tokens = (await _context.Tokens.Where(e => e.UserId == user.Id).ToListAsync()).Adapt<List<TokenDto>>();

        return Ok(tokens);
    }

    [HttpGet("tokens/token-name={tokenName}")]
    public async Task<IActionResult> GetTokenByName(string tokenname, string username)
    {
        var user = await _context.Users.SingleAsync(e => e.UserName == username);

        if (user == null)
            throw new UserNotFoundException();

        var token = 
            _context.Tokens.SingleAsync(e => e.Name == tokenname 
            && e.UserId == user.Id);

        if (token == null)
        {
            throw new TokenNotFoundException();
        }

        var resultToken = token.Adapt<TokenDto>();

        return Ok(resultToken);
    }
}