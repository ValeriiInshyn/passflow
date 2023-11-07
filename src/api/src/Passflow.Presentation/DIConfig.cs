using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Passflow.Infrastructure.Database;
using System.Reflection;

namespace Passflow.Presentation
{
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

        public static void AddSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(options =>
            {
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

                // options.SchemaFilter<EnumSchemaFilter>();
                options.UseInlineDefinitionsForEnums();

                options.SupportNonNullableReferenceTypes(); // Sets Nullable flags appropriately.              
                options.UseAllOfForInheritance(); // Allows $ref objects to be nullable
            });


            services.AddRouting(options => options.LowercaseUrls = true);
        }
    }
}
