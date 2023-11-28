using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Passflow.Domain.Settings;
using Passflow.Infrastructure.Database;
using Passflow.Infrastructure.Middlewares;
using Passflow.Infrastructure.Services;
using Passflow.Presentation;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSerilog(builder.Configuration);
builder.Services.AddConfigs(builder.Configuration);
builder.Services.AddSingleton(Log.Logger);

builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger(builder.Configuration);
builder.Services.AddServices();
builder.Host.UseSerilog();
builder.Services.AddCustomAuthentication(builder.Configuration);
var app = builder.Build();


await using (var scope = app.Services.CreateAsyncScope())
{
	var services = scope.ServiceProvider;
	await using var dbContext = services.GetRequiredService<PassflowDbContext>();
	//await dbContext.Database.MigrateAsync();
	//await dbContext.SeedDataAsync();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseRouting();
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
