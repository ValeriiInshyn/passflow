using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace Passflow.Contracts.Dtos;

public sealed record AdminDto
{
    [SwaggerSchema("The admin's user name. Must be unique")]
    public string UserName { get; set; } = null!;
    
    [SwaggerSchema("The admin's email. Must be unique")]
    public string? Email { get; set; }

    [SwaggerSchema("The admin's hash password.")]
    public string PasswordHash { get; set; } = null!;
}