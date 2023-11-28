using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Passflow.Domain.Settings;
using Passflow.Infrastructure.Database;
using Passflow.Infrastructure.Services;
using Serilog;
using System.Reflection;
using Passflow.Application.Services;
using Passflow.Presentation.OperationFilters;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Passflow.Presentation;

public static class DIConfig
{
	public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddDbContext<PassflowDbContext>(
			options =>
			{
				var connectionString = configuration.GetValue<string>("ConnectionString");
				options.UseSqlServer(connectionString);
			});

		return services;
	}

	public static IServiceCollection AddConfigs(this IServiceCollection services, IConfiguration configuration)
	{
		services.Configure<JwtAuthSettings>(configuration.GetRequiredSection(nameof(JwtAuthSettings)));
		return services;
	}
	#region Logger

	public static IServiceCollection AddSerilog(this IServiceCollection services, IConfiguration configuration)
	{
		Log.Logger = new LoggerConfiguration()
			.ReadFrom.Configuration(configuration)
			.CreateLogger();
		return services;
	}

	#endregion

	public static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddSwaggerGen(options =>
		{
			var jwtSecurityScheme = new OpenApiSecurityScheme
			{
				BearerFormat = "JWT",
				Name = "JWT Authentication",
				In = ParameterLocation.Header,
				Type = SecuritySchemeType.Http,
				Scheme = JwtBearerDefaults.AuthenticationScheme,
				Description = "Put ONLY your JWT Bearer token in text box below!",
				Reference = new OpenApiReference
				{
					Id = JwtBearerDefaults.AuthenticationScheme,
					Type = ReferenceType.SecurityScheme
				}
			};
			options.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
			options.AddSecurityRequirement(new OpenApiSecurityRequirement { { jwtSecurityScheme, Array.Empty<string>() } });
			var swaggerSection = configuration.GetSection("Swagger");
			var licenseSection = swaggerSection.GetSection("License");

			var currentAssembly = Assembly.GetExecutingAssembly();
			var xmlDocs = currentAssembly.GetReferencedAssemblies()
				.Union(new[] { currentAssembly.GetName() })
				.Select(a => Path.Combine(Path.GetDirectoryName(currentAssembly.Location)!,
					$"{a.Name}.xml"))
				.Where(File.Exists).ToArray();
			Array.ForEach(xmlDocs, d => { options.IncludeXmlComments(d); });


			options.EnableAnnotations(true, true);

			options.OrderActionsBy(apiDesc =>
				$"{apiDesc.ActionDescriptor.RouteValues["controller"]}_{apiDesc.HttpMethod}_{apiDesc.RelativePath}");

			options.UseInlineDefinitionsForEnums();
			options.OperationFilter<MethodNameAsOperationIdFilter>();
			options.SupportNonNullableReferenceTypes(); // Sets Nullable flags appropriately.              
			options.UseAllOfForInheritance(); // Allows $ref objects to be nullable
		});


		services.AddRouting(options => options.LowercaseUrls = true);

		return services;
	}

	public static IServiceCollection AddCustomAuthentication(this IServiceCollection services,
		IConfiguration configuration)
	{
		services.AddAuthentication(options =>
		{
			options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
		}).AddJwtBearer(o =>
		{
			var jwtAuthSettings = configuration.GetRequiredSection(nameof(JwtAuthSettings)).Get<JwtAuthSettings>();
			o.TokenValidationParameters = TokenService.GetAccessTokenValidationParameters(jwtAuthSettings!);
			o.Events = new JwtBearerEvents
			{
				OnChallenge = async context =>
				{
					context.HandleResponse();
					context.Response.StatusCode = 401;
					context.Response.ContentType = "application/json";
					if (context.AuthenticateFailure?.GetType() == typeof(SecurityTokenExpiredException))
					{
						await context.Response.WriteAsJsonAsync(
							new SecurityTokenExpiredException("Token expired"));
						return;
					}

					await context.Response.WriteAsync("Not authorized");
				},
				// OnForbidden = context => { },
				OnAuthenticationFailed = context =>
				{
					if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
					{
						context.Response.Headers.Add("Token-Expired", "true");
					}

					return Task.CompletedTask;
				}
			};
		});
		return services;
	}

	public static IServiceCollection AddServices(this IServiceCollection services)
	{
		services.AddSingleton<ITokenService, TokenService>();
		return services;
	}

}