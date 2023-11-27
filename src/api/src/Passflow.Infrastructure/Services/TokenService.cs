using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Passflow.Application.Services;
using Passflow.Domain.Settings;

namespace Passflow.Infrastructure.Services;
public class TokenService:ITokenService
{
    private readonly JwtAuthSettings _jwtAuthSettings;

    public TokenService(IOptions<JwtAuthSettings> jwtAuthSettings)
    {
        _jwtAuthSettings = jwtAuthSettings.Value;
    }
    public string GenerateAccessToken(string username, string role)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(ClaimTypes.Name, username),
                new(ClaimTypes.Role,role)
            }),
            Issuer = _jwtAuthSettings.Issuer,
            IssuedAt = DateTime.UtcNow,
            TokenType = JwtBearerDefaults.AuthenticationScheme,
            Expires = DateTime.UtcNow.AddMinutes(_jwtAuthSettings.AccessTokenLifetimeInMinutes),
            SigningCredentials = GetSigningCredentials(_jwtAuthSettings.AccessSecret)
        };

        var token = tokenHandler.CreateEncodedJwt(tokenDescriptor);
        return token;
    }
    public string GenerateRefreshToken(string username)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new(ClaimTypes.Name, username),
            }),
            TokenType = JwtBearerDefaults.AuthenticationScheme,
            Expires = DateTime.UtcNow.AddDays(_jwtAuthSettings.RefreshTokenLifetimeInDays),
            SigningCredentials = GetSigningCredentials(_jwtAuthSettings.RefreshSecret)
        };

        var token = tokenHandler.CreateEncodedJwt(tokenDescriptor);
        return token;
    }

    public static TokenValidationParameters GetAccessTokenValidationParameters(JwtAuthSettings settings)
    {
        return new TokenValidationParameters
        {
            IssuerSigningKey = GetSymmetricSecurityKey(settings.AccessSecret),
            ClockSkew = TimeSpan.FromSeconds(30),
            ValidIssuer = settings.Issuer,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
        };
    }

    /// <summary>
    /// Reads and validates a 'JSON Web Token' (JWT) encoded as a JWS or JWE in Compact Serialized Format.
    /// </summary>
    /// <param name="token">the JWT encoded as JWE or JWS</param>
    /// <param name="validationParameters">Contains validation parameters for the <see cref="JwtSecurityToken"/>.</param>
    /// <param name="validatedToken">The <see cref="JwtSecurityToken"/> that was validated.</param>
    /// <exception cref="ArgumentNullException"><paramref name="token"/> is null or whitespace.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="validationParameters"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="token"/>.Length is greater than <see cref="TokenHandler.MaximumTokenSizeInBytes"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="token"/> does not have 3 or 5 parts.</exception>
    /// <exception cref="ArgumentException"><see cref="CanReadToken(string)"/> returns false.</exception>
    /// <exception cref="SecurityTokenDecryptionFailedException"><paramref name="token"/> was a JWE was not able to be decrypted.</exception>
    /// <exception cref="SecurityTokenEncryptionKeyNotFoundException"><paramref name="token"/> 'kid' header claim is not null AND decryption fails.</exception>
    /// <exception cref="SecurityTokenException"><paramref name="token"/> 'enc' header claim is null or empty.</exception>
    /// <exception cref="SecurityTokenExpiredException"><paramref name="token"/> 'exp' claim is &lt; DateTime.UtcNow.</exception>
    /// <exception cref="SecurityTokenInvalidAudienceException"><see cref="TokenValidationParameters.ValidAudience"/> is null or whitespace and <see cref="TokenValidationParameters.ValidAudiences"/> is null. Audience is not validated if <see cref="TokenValidationParameters.ValidateAudience"/> is set to false.</exception>
    /// <exception cref="SecurityTokenInvalidAudienceException"><paramref name="token"/> 'aud' claim did not match either <see cref="TokenValidationParameters.ValidAudience"/> or one of <see cref="TokenValidationParameters.ValidAudiences"/>.</exception>
    /// <exception cref="SecurityTokenInvalidLifetimeException"><paramref name="token"/> 'nbf' claim is &gt; 'exp' claim.</exception>
    /// <exception cref="SecurityTokenInvalidSignatureException"><paramref name="token"/>.signature is not properly formatted.</exception>
    /// <exception cref="SecurityTokenNoExpirationException"><paramref name="token"/> 'exp' claim is missing and <see cref="TokenValidationParameters.RequireExpirationTime"/> is true.</exception>
    /// <exception cref="SecurityTokenNoExpirationException"><see cref="TokenValidationParameters.TokenReplayCache"/> is not null and expirationTime.HasValue is false. When a TokenReplayCache is set, tokens require an expiration time.</exception>
    /// <exception cref="SecurityTokenNotYetValidException"><paramref name="token"/> 'nbf' claim is &gt; DateTime.UtcNow.</exception>
    /// <exception cref="SecurityTokenReplayAddFailedException"><paramref name="token"/> could not be added to the <see cref="TokenValidationParameters.TokenReplayCache"/>.</exception>
    /// <exception cref="SecurityTokenReplayDetectedException"><paramref name="token"/> is found in the cache.</exception>
    /// <returns> A <see cref="ClaimsPrincipal"/> from the JWT. Does not include claims found in the JWT header.</returns>
    /// <remarks> 
    /// Many of the exceptions listed above are not thrown directly from this method. See <see cref="Validators"/> to examine the call graph.
    /// </remarks>
    public ClaimsPrincipal ValidateAccessToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = GetAccessTokenValidationParameters(_jwtAuthSettings);
        return tokenHandler.ValidateToken(token, validationParameters, out _);
    }
    public static TokenValidationParameters GetRefreshTokenValidationParameters(JwtAuthSettings settings)
    {
        return new TokenValidationParameters
        {
            IssuerSigningKey = GetSymmetricSecurityKey(settings.RefreshSecret),
            ClockSkew = TimeSpan.FromSeconds(30),
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
        };
    }
    /// <summary>
    /// Reads and validates a 'JSON Web Token' (JWT) encoded as a JWS or JWE in Compact Serialized Format.
    /// </summary>
    /// <param name="token">the JWT encoded as JWE or JWS</param>

    /// <exception cref="ArgumentException"><paramref name="token"/>.Length is greater than <see cref="TokenHandler.MaximumTokenSizeInBytes"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="token"/> does not have 3 or 5 parts.</exception>
    /// <exception cref="SecurityTokenDecryptionFailedException"><paramref name="token"/> was a JWE was not able to be decrypted.</exception>
    /// <exception cref="SecurityTokenEncryptionKeyNotFoundException"><paramref name="token"/> 'kid' header claim is not null AND decryption fails.</exception>
    /// <exception cref="SecurityTokenException"><paramref name="token"/> 'enc' header claim is null or empty.</exception>
    /// <exception cref="SecurityTokenExpiredException"><paramref name="token"/> 'exp' claim is &lt; DateTime.UtcNow.</exception>
    /// <exception cref="SecurityTokenInvalidAudienceException"><see cref="TokenValidationParameters.ValidAudience"/> is null or whitespace and <see cref="TokenValidationParameters.ValidAudiences"/> is null. Audience is not validated if <see cref="TokenValidationParameters.ValidateAudience"/> is set to false.</exception>
    /// <exception cref="SecurityTokenInvalidAudienceException"><paramref name="token"/> 'aud' claim did not match either <see cref="TokenValidationParameters.ValidAudience"/> or one of <see cref="TokenValidationParameters.ValidAudiences"/>.</exception>
    /// <exception cref="SecurityTokenInvalidLifetimeException"><paramref name="token"/> 'nbf' claim is &gt; 'exp' claim.</exception>
    /// <exception cref="SecurityTokenInvalidSignatureException"><paramref name="token"/>.signature is not properly formatted.</exception>
    /// <exception cref="SecurityTokenNoExpirationException"><paramref name="token"/> 'exp' claim is missing and <see cref="TokenValidationParameters.RequireExpirationTime"/> is true.</exception>
    /// <exception cref="SecurityTokenNoExpirationException"><see cref="TokenValidationParameters.TokenReplayCache"/> is not null and expirationTime.HasValue is false. When a TokenReplayCache is set, tokens require an expiration time.</exception>
    /// <exception cref="SecurityTokenNotYetValidException"><paramref name="token"/> 'nbf' claim is &gt; DateTime.UtcNow.</exception>
    /// <exception cref="SecurityTokenReplayAddFailedException"><paramref name="token"/> could not be added to the <see cref="TokenValidationParameters.TokenReplayCache"/>.</exception>
    /// <exception cref="SecurityTokenReplayDetectedException"><paramref name="token"/> is found in the cache.</exception>
    /// <returns> A <see cref="ClaimsPrincipal"/> from the JWT. Does not include claims found in the JWT header.</returns>
    /// <remarks> 
    /// Many of the exceptions listed above are not thrown directly from this method. See <see cref="Validators"/> to examine the call graph.
    /// </remarks>
    public ClaimsPrincipal ValidateRefreshToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = GetRefreshTokenValidationParameters(_jwtAuthSettings);

        return tokenHandler.ValidateToken(token, validationParameters, out _);
    }
    private static SigningCredentials GetSigningCredentials(string secret)
    {
        return new SigningCredentials(GetSymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
    }
    private static SymmetricSecurityKey GetSymmetricSecurityKey(string secret)
    {
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
    }
}
