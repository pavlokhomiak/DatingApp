using API.Extensions;
using API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

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
app.Run();
