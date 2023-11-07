using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Passflow.Infrastructure.Database;
using Mapster;
using Passflow.Contracts.Dtos;
using Passflow.Contracts.Exceptions;
using Passflow.Domain;
using Swashbuckle.AspNetCore.Annotations;

namespace Passflow.Presentation.Controllers;

public class UsersController : BaseApiController
{
    private readonly PassflowDbContext _context;

    public UsersController(PassflowDbContext context)
    {
        _context = context;
    }



    [SwaggerOperation(
        Summary = "Gets all users",
        Description = "Gets all users",
        OperationId = nameof(List<UserDto>)
    )]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "All users successfully loaded"
    )]

    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = (await _context.Users.ToListAsync()).Adapt<List<UserDto>>();

        return Ok(users);
    }



    [SwaggerOperation(
        Summary = "Gets user by name",
        Description = "Gets one user from users collection by it's unique username",
        OperationId = nameof(UserDto)
    )]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "User successfully loaded"
    )]
    [HttpGet("users/name={username}")]
    public async Task<IActionResult> GetUserByName([FromRoute] string username)
    {
        var user = await _context.Users.SingleAsync(e => e.UserName == username);

        if (user == null)
            throw new UserNotFoundException();

        var response = user.Adapt<UserDto>();

        return Ok(response);
    }

    [SwaggerOperation(
        Summary = "Create user",
        Description = "Creates user using user's model",
        OperationId = nameof(UserDto)
    )]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "User successfully loaded"
    )]
    [HttpGet("users/create")]
    public async Task<IActionResult> CreateUser(UserDto userDto)
    {
        var user = await _context.Users.FirstAsync(e => e.UserName == userDto.UserName);

        if (user == null)
            throw new UserAlreadyExistsException();

        var response = user.Adapt<UserDto>();

        return Ok(response);
    }

    [SwaggerOperation(
        Summary = "Updates users model",
        Description = "Gets user from users collection and updates user properties",
        OperationId = nameof(UserDto)
    )]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "User successfully updated"
    )]
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



    [SwaggerOperation(
        Summary = "Deletes users model",
        Description = "Gets user from users collection and deletes him",
        OperationId = nameof(UserDto)
    )]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "User successfully deleted"
    )]
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



    [SwaggerOperation(
        Summary = "Paginates users collection",
        Description = "Gets users from users collection",
    OperationId = nameof(UserDto)
    )]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "Users successfully loaded"
    )]
    [HttpGet("users/skip={skip}take={take}")]
    public async Task<IActionResult> Paginate([FromRoute]int skip, [FromRoute] int take)
    {
        var users = (await _context.Users.Skip(skip).Take(take).ToListAsync()).Adapt<List<UserDto>>();

        return Ok(users);
    }
}