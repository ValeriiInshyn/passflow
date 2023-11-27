using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passflow.Contracts.Dtos.User;

public class UserCreateDto
{
	public string UserName { get; set; } = null!;
	public string Password { get; set; } = null!;
	public string? FirstName { get; set; }
	public string? LastName  { get; set; }
	public string? Email { get; set; }
}
