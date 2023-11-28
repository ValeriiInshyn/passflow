using Azure;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Passflow.Contracts.Dtos;
using Passflow.Contracts.Exceptions;
using Passflow.Domain;
using Passflow.Infrastructure.Database;
using Swashbuckle.AspNetCore.Annotations;

namespace Passflow.Presentation.Controllers
{
    public class GroupsController : BaseApiController
    {

        private readonly PassflowDbContext _context;

        public GroupsController(PassflowDbContext context)
        {
            _context = context;
        }



        [SwaggerOperation(
            Summary = "Gets all groups",
            Description = "Gets all groups"
        )]
        [SwaggerResponse(
            StatusCodes.Status200OK,
            "All groups successfully loaded",
            typeof(List<GroupDto>)
        )]
        [HttpGet]
        public async Task<IActionResult> GetAllGroupsAsync()
        {
            var groups = (await _context.Groups.ToListAsync()).Adapt<List<GroupDto>>();

            return Ok(groups);
        }



        [SwaggerOperation(
            Summary = "Gets group by name",
            Description = "Gets one group from groups collection by it's unique groupName"
        )]
        [SwaggerResponse(
            StatusCodes.Status200OK,
            "group successfully loaded",
            typeof(GroupDto)
		)]
        [HttpGet("name={groupName}")]
        public async Task<IActionResult> GetGroupByNameAsync([FromRoute] string groupname)
        {
            var group = await _context.Groups.SingleOrDefaultAsync(e => e.GroupName == groupname);

            if (group == null)
                throw new UserNotFoundException($"Group with name {groupname} not found!");

            var response = group.Adapt<GroupDto>();

            return Ok(response);
        }

        [SwaggerOperation(
            Summary = "Create group",
            Description = "Creates group using group's model"
        )]
        [SwaggerResponse(
            StatusCodes.Status200OK,
            "Group successfully loaded",
            typeof(string)
        )]
        [HttpPost("create")]
        public async Task<IActionResult> CreateGroup(GroupDto groupDto)
        {
            var group = await _context.Groups.FirstOrDefaultAsync(e => e.GroupName == groupDto.GroupName);

            if (group is not null)
                throw new UserAlreadyExistsException($"Group with name {groupDto.GroupName} already exist!");

            group = groupDto.Adapt<Group>();

            await _context.Groups.AddAsync(group);
            await _context.SaveChangesAsync();

            return Ok($"{groupDto.GroupName} successfully created!");
        }


        [SwaggerOperation(
            Summary = "Updates groups model",
            Description = "Gets group from groups collection and updates group properties"
        )]
        [SwaggerResponse(
            StatusCodes.Status200OK,
            "group successfully updated",
            typeof(GroupDto)
        )]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateGroup(GroupDto groupDto)
        {
            var group = await _context.Groups.SingleOrDefaultAsync(e => e.GroupName == groupDto.GroupName);

            if (group == null)
                throw new UserNotFoundException($"Group with name {groupDto.GroupName} not found!");

            group = groupDto.Adapt<Group>();

            _context.Groups.Update(group);
            await _context.SaveChangesAsync();
            return Ok(groupDto);
        }



        [SwaggerOperation(
            Summary = "Deletes groups model",
            Description = "Gets group from groups collection and deletes him"
        )]
        [SwaggerResponse(
            StatusCodes.Status200OK,
            "Group successfully deleted",
            typeof(string)
        )]
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteGroupAsync(string groupName)
        {
            var group = await _context.Groups.SingleOrDefaultAsync(e => e.GroupName == groupName);

            if (group is null)
                throw new GroupNotFoundException($"Group with name {groupName} not found!");

            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();


            return Ok($"{groupName} successfully deleted!");
        }



        [SwaggerOperation(
            Summary = "Paginates groups collection",
            Description = "Gets groups from groups collection",
        OperationId = nameof(GroupDto)
        )]
        [SwaggerResponse(
            StatusCodes.Status200OK,
            "groups successfully loaded",
            typeof(List<GroupDto>)
        )]
        [HttpGet("skip={skip}take={take}")]
        public async Task<IActionResult> PaginateGroupsAsync([FromRoute] int skip, [FromRoute] int take)
        {
            var groups = (await _context.Groups.Skip(skip).Take(take).ToListAsync()).Adapt<List<GroupDto>>();

            return Ok(groups);
        }
    }
}
