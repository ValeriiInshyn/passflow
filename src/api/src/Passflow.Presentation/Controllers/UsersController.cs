using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Passflow.Infrastructure.Database;
using Mapster;
using Passflow.Contracts.Dtos;
using Passflow.Contracts.Exceptions;
using Passflow.Domain;

namespace Passflow.Presentation.Controllers;

public class UsersController : BaseApiController
{
    private readonly PassflowDbContext _context;

    public UsersController(PassflowDbContext context)
    {
        _context = context;
    }
    
    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = (await _context.Users.ToListAsync()).Adapt<List<UserDto>>();

        return Ok(users);
    }

    [HttpGet("users/name={username}")]
    public async Task<IActionResult> GetUserByName([FromRoute] string username)
    {
        var user = await _context.Users.SingleAsync(e => e.UserName == username);

        if (user == null)
            throw new UserNotFoundException();

        var response = user.Adapt<UserDto>();

        return Ok(response);
    }

    [HttpPut("users/update")]
    public async Task<IActionResult> UpdateUser(UserDto userDto)
    {
        var user = await _context.Users.SingleAsync(e => e.UserName == userDto.UserName);

        if (user == null)
            throw new UserNotFoundException();

        user = userDto.Adapt<User>();

        _context.Users.Update(user);
        await _context.SaveChangesAsync();
           

        return Ok(userDto);
    }

    [HttpDelete("users/delete")]
    public async Task<IActionResult> DeleteUser(string username)
    {
        var user = await _context.Users.SingleAsync(e => e.UserName == username);

        if (user == null)
            throw new UserNotFoundException();

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();


        return Ok($"{username} successfully deleted!");
    }

    [HttpGet("users/skip={skip}take={take}")]
    public async Task<IActionResult> Paginate([FromRoute]int skip, [FromRoute] int take)
    {
        var users = (await _context.Users.Skip(skip).Take(take).ToListAsync()).Adapt<List<UserDto>>();

        return Ok(users);
    }
}