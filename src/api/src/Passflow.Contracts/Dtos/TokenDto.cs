using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passflow.Contracts.Dtos;

public sealed record TokenDto
{
    public string Name { get; set; } = null!;

    public string Value { get; set; } = null!;
}