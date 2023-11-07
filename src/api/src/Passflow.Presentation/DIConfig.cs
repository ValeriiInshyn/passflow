using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Passflow.Infrastructure.Database;

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
    }
}
