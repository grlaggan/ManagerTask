using ManagerTask.Application.Abstracts;
using ManagerTask.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TransactionManager = ManagerTask.Infrastructure.Common.TransactionManager;

namespace ManagerTask.Infrastructure;

public static class InfrastructureDi
{
    private const string PostgresConnection = "DefaultConnection";

    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructure(IConfiguration configuration)
        {
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<ITableRepository, TableRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITransactionManager, TransactionManager>();
            services.AddDbContext<IApplicationDbContext, ApplicationDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString(PostgresConnection));
            });

            return services;
        }
    }
}