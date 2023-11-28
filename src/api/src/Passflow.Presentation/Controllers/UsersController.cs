using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Passflow.Infrastructure.Database;
using Mapster;
using Passflow.Contracts.Exceptions;
using Passflow.Domain;
using Swashbuckle.AspNetCore.Annotations;
using Passflow.Contracts.Dtos.User;
using Microsoft.AspNetCore.Authorization;

namespace Passflow.Presentation.Controllers;
[Authorize]
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
        "All users successfully loaded",
        typeof(List<UserDto>)
    )]

    [HttpGet]
    public async Task<IActionResult> GetAllUsersAsync()
    {
        var users = (await _context.Users.ToListAsync()).Adapt<List<UserDto>>();

        return Ok(users);
    }



    [SwaggerOperation(
        Summary = "Gets user by name",
        Description = "Gets one user from users collection by it's unique username"
    )]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "User successfully loaded",
        typeof(UserDto)
    )]
    [HttpGet("{username}")]
    public async Task<IActionResult> GetUserByNameAsync([FromRoute] string username)
    {
        var user = await _context.Users.SingleOrDefaultAsync(e => e.UserName == username);

        if (user == null)
            throw new UserNotFoundException($"User with name {username} not found!");

        var response = user.Adapt<UserDto>();

        return Ok(response);
    }

    [SwaggerOperation(
        Summary = "Create user",
        Description = "Creates user using user's model"
    )]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "User successfully loaded",
        typeof(string)
    )]
    [HttpPost("create")]
    public async Task<IActionResult> CreateUserAsync(UserCreateDto userDto)
    {
        var user = await _context.Users.SingleOrDefaultAsync(e => e.UserName == userDto.UserName);
        if (user is not null)
            throw new UserAlreadyExistsException($"User with name {userDto.UserName} already exist!");

        user = userDto.Adapt<User>();
		await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return Ok($"{userDto.UserName} successfully created!");
    }

    [SwaggerOperation(
        Summary = "Updates users model",
        Description = "Gets user from users collection and updates user properties"
    )]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "User successfully updated",
        typeof(UserDto)
    )]
    [HttpPut("update")]
    public async Task<IActionResult> UpdateUserAsync(UserDto userDto)
    {
        var user = await _context.Users.SingleOrDefaultAsync(e => e.UserName == userDto.UserName);

        if (user == null)
            throw new UserNotFoundException($"User with name {userDto.UserName} not found!");

        user = userDto.Adapt<User>();

        _context.Users.Update(user);
        await _context.SaveChangesAsync();
           

        return Ok(userDto);
    }



    [SwaggerOperation(
        Summary = "Deletes users model",
        Description = "Gets user from users collection and deletes him"
    )]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "User successfully deleted",
        typeof(string)
    )]
    [HttpDelete("{username}")]
    public async Task<IActionResult> DeleteUserAsync(string username)
    {
        var user = await _context.Users.SingleOrDefaultAsync(e => e.UserName == username);

        if (user == null)
            throw new UserNotFoundException($"User with name {username} not found!");

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
        "Users successfully loaded",
        typeof(List<UserDto>)
	)]
    [HttpGet("{skip:int}/{take:int}")]
	public async Task<IActionResult> PaginateUsersAsync([FromRoute]int skip, [FromRoute] int take)
    {
        var users = (await _context.Users.Skip(skip).Take(take).ToListAsync()).Adapt<List<UserDto>>();

        return Ok(users);
    }
}