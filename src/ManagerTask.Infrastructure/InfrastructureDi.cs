using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ManagerTask.Infrastructure;

public static class InfrastructureDi
{
    private const string PostgresConnection = "DefaultConnection";
    
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructure(IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString(PostgresConnection));
            });
            
            return services;
        }
    }
}