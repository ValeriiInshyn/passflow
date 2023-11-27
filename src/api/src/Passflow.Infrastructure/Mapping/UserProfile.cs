using Mapster;
using Passflow.Contracts.Dtos.User;
using Passflow.Infrastructure.Services;

namespace Passflow.Infrastructure.Mapping;
internal class UserProfile : IRegister

{
	public void Register(TypeAdapterConfig config)
	{
		config.NewConfig<UserCreateDto, UserDto>()
			.Map(dest => dest.PasswordHash, src => PasswordEncrypter.HashPassword(src.Password));
	}
}
