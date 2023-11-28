using Microsoft.AspNetCore.Mvc;

namespace Passflow.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class BaseApiController : ControllerBase
{
	protected string GetAuthUserName() => User.Identity!.Name!;

}