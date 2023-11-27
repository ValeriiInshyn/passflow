using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Passflow.Infrastructure.Database;
using Passflow.Infrastructure.Middlewares;
using Passflow.Presentation;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSerilog(builder.Configuration);

builder.Services.AddSingleton(Log.Logger);

// Add services to the container.
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger(builder.Configuration);

builder.Host.UseSerilog();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<PassflowDbContext>();
    //await Task.Delay(15000);
  //  dbContext.Database.Migrate();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
