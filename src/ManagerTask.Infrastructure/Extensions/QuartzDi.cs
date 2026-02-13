using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace ManagerTask.Infrastructure.Extensions;

public static class QuartzDi
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddQuartzDi()
        {
            services.AddQuartz();

            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

            return services;
        }
    }
}
