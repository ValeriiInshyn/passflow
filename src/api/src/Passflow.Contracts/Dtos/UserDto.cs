using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passflow.Contracts.Dtos;

public sealed record UserDto
{
    [SwaggerSchema("The user's name. Must be unique.")]
    public string UserName { get; set; } = null!;

    [SwaggerSchema("The user's first name.")]
    public string? FirstName { get; set; }

    [SwaggerSchema("The user's last name.")]
    public string? LastName { get; set; }

    [SwaggerSchema("The user's email. Must be unique.")]
    public string? Email { get; set; }

    [SwaggerSchema("The user's hash password.")]
    public string PasswordHash { get; set; } = null!;
}