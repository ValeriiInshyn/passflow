using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passflow.Contracts.Dtos;

public sealed record AdminDto
{
    public string UserName { get; set; } = null!;

    public string? Email { get; set; }

    public string PasswordHash { get; set; } = null!;
}