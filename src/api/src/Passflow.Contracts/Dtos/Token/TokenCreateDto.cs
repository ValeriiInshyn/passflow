using Swashbuckle.AspNetCore.Annotations;


namespace Passflow.Contracts.Dtos.Token
{
    public class TokenCreateDto
    {

        [SwaggerSchema("The token's name.")]
        public string Name { get; set; } = null!;

        [SwaggerSchema("The token's value.")]
        public string Value { get; set; } = null!;
    }
}
