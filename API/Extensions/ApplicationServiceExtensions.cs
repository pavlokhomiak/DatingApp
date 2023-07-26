using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    // extracted from Program.cs
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config) 
        {
            services.AddDbContext<DataContext>(opt => 
                {
                    // opt.UseSqlite(config.GetConnectionString("SqliteConnection"));
                    opt.UseNpgsql(config.GetConnectionString("PostgresConnection"));
                    // Solve System.InvalidCastException: Cannot write DateTime with Kind=Unspecified to PostgreSQL type 'timestamp with time zone', only UTC is supported.
                    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
                });
            services.AddCors();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserRepository, UserRepository>();
            // we add this because we need to use automapper in our controllers
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
            services.AddScoped<IPhotoService, PhotoService>();
            services.AddScoped<LogUserActivity>();
            services.AddScoped<ILikesRepository, LikesRepository>();

            return services;
        }
    }
}