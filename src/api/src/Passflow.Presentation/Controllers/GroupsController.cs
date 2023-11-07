using Microsoft.AspNetCore.Mvc;

namespace Passflow.Presentation.Controllers
{
    public class GroupsController : Controller
    {

        public GroupsController()
        {

        }
        [HttpGet("groups")]
        public async Task<IActionResult> GetAllGroups()
        {
            return Ok("Good");
        }
    }
}
