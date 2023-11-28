using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WPFApp.Nswag;

namespace WPF_app.Nswag;
public class ApiClient : NswagClient
{
	public static ApiClient Instance { get; private set; } = new(BaseUrl, new HttpClient());
	private const string BaseUrl = "https://localhost:7267";
	private string? _refreshToken;
	public string? UserName { get; private set; }
	public override async Task<TokenResponse> LoginAsync(LoginDto body)
	{
		var loginResponse = await base.LoginAsync(body);
		var httpClient = new HttpClient();
		httpClient.DefaultRequestHeaders.Authorization =
			new AuthenticationHeaderValue("Bearer", loginResponse.AccessToken);
		_refreshToken = loginResponse.RefreshToken;
		UserName = body.UserName;
		Instance = new ApiClient(BaseUrl, httpClient);
		return loginResponse;
	}

	public ApiClient(string baseUrl, HttpClient httpClient) : base(baseUrl, httpClient)
	{
	}
}
