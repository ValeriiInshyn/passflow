using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passflow.Contracts.Dtos;

public sealed record GroupDto
{
    [SwaggerSchema("The group's group name. Must be unique")]
    public string GroupName { get; set; } = null!;
}