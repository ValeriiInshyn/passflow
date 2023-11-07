using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passflow.Contracts.Dtos;

public sealed record TokenDto
{
    [SwaggerSchema("The token's name.")]
    public string Name { get; set; } = null!;

    [SwaggerSchema("The token's value.")]
    public string Value { get; set; } = null!;
}