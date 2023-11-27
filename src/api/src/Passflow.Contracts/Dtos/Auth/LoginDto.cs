using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passflow.Contracts.Dtos.Auth;
public class LoginDto
{
	[SwaggerSchema("UserName to login")]
	public string UserName { get; set; } = null!;
	[SwaggerSchema("Password to login")]
	public string Password { get; set; } = null!;
}
