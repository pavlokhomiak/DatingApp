using API.Data;
using API.Extensions;
using API.Middleware;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

// Add middleware
var app = builder.Build();

// dotnet 5
// if (builder.Environment.IsDevelopment()) {
//     app.UseDeveloperExceptionPage();
// }

// Configure the HTTP request pipeline.

// switch on error handling
app.UseMiddleware<ExceptionMiddleware>();
// fix cors error for angular client call 
app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200"));
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var context = services.GetRequiredService<DataContext>();
    await context.Database.MigrateAsync();
    await Seed.SeedUsers(context);
} catch (Exception e) {
    var logger = services.GetService<ILogger<Program>>();
    logger.LogError(e, "Error during migration");
}

app.Run();
