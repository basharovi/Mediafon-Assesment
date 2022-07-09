using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MediafonTech.Infrastructure.Context;
using MediafonTech.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using MediafonTech.Infrastructure.Repositories;
using MediafonTech.Infrastructure.Configurations;
using MediafonTech.ApplicationCore.Interfaces.Services;
using MediafonTech.ApplicationCore.Interfaces.Repositories;


namespace MediafonTech.Infrastructure
{
    public static class ServiceRegistration
    {
        public static void RegisterInfrastructure(this IServiceCollection services,
            IConfiguration configuration)
        {
            // Kept all the dependency ralated code like a module.
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("PostgreSql"),
                    a => a.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));
            })
            .AddTransient<IDataSyncService, DataSyncService>()
            .AddTransient<IFileInfoRepository, FileInfoRepository>()
            .Configure<FtpConfig>(configuration.GetSection("FtpConfig"))
            .AddTransient(r => r.GetRequiredService<IOptions<FtpConfig>>().Value);

            // Added for storing timestamp without timezone
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }
    }
}
