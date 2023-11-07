using Azure;
using Mapster;
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
            Description = "Gets all groups",
            OperationId = nameof(List<GroupDto>)
        )]
        [SwaggerResponse(
            StatusCodes.Status200OK,
            "All groups successfully loaded"
        )]

        [HttpGet("groups")]
        public async Task<IActionResult> GetAllgroups()
        {
            var groups = (await _context.Groups.ToListAsync()).Adapt<List<GroupDto>>();

            return Ok(groups);
        }



        [SwaggerOperation(
            Summary = "Gets group by name",
            Description = "Gets one group from groups collection by it's unique groupname",
            OperationId = nameof(GroupDto)
        )]
        [SwaggerResponse(
            StatusCodes.Status200OK,
            "group successfully loaded"
        )]
        [HttpGet("groups/name={groupname}")]
        public async Task<IActionResult> GetGroupByName([FromRoute] string groupname)
        {
            var group = await _context.Groups.SingleAsync(e => e.GroupName == groupname);

            if (group == null)
                throw new GroupNotFoundException();

            var response = group.Adapt<GroupDto>();

            return Ok(response);
        }



        [SwaggerOperation(
            Summary = "Updates groups model",
            Description = "Gets group from groups collection and updates group properties",
            OperationId = nameof(groupDto)
        )]
        [SwaggerResponse(
            StatusCodes.Status200OK,
            "group successfully updated"
        )]
        [HttpPut("groups/update")]
        public async Task<IActionResult> Updategroup(GroupDto groupDto)
        {
            var group = await _context.Groups.SingleAsync(e => e.GroupName == groupDto.GroupName);

            if (group == null)
                throw new GroupNotFoundException();

            group = groupDto.Adapt<Group>();

            _context.Groups.Update(group);
            await _context.SaveChangesAsync();
            return Ok(groupDto);
        }



        [SwaggerOperation(
            Summary = "Deletes groups model",
            Description = "Gets group from groups collection and deletes him",
            OperationId = nameof(GroupDto)
        )]
        [SwaggerResponse(
            StatusCodes.Status200OK,
            "group successfully deleted"
        )]
        [HttpDelete("groups/delete")]
        public async Task<IActionResult> Deletegroup(string groupname)
        {
            var group = await _context.Groups.SingleAsync(e => e.GroupName == groupname);

            if (group == null)
                throw new GroupNotFoundException();

            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();


            return Ok($"{groupname} successfully deleted!");
        }



        [SwaggerOperation(
            Summary = "Paginates groups collection",
            Description = "Gets groups from groups collection",
        OperationId = nameof(GroupDto)
        )]
        [SwaggerResponse(
            StatusCodes.Status200OK,
            "groups successfully loaded"
        )]
        [HttpGet("groups/skip={skip}take={take}")]
        public async Task<IActionResult> Paginate([FromRoute] int skip, [FromRoute] int take)
        {
            var groups = (await _context.Groups.Skip(skip).Take(take).ToListAsync()).Adapt<List<GroupDto>>();

            return Ok(groups);
        }
    }
}
