using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passflow.Contracts.Dtos.Auth;

public class TokenResponse
{
	public string AccessToken { get; set; } = null!;
	public string RefreshToken { get; set; } = null!;
}
