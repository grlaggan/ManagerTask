using ManagerTask.Application.Abstracts;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ManagerTask.HealthChecks;

public class DatabaseHealthCheck(IApplicationDbContext dbContext) : IHealthCheck
{

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        var canConnect = await dbContext.Database.CanConnectAsync(cancellationToken);
        
        return canConnect ? HealthCheckResult.Healthy("RabbitMq connection is OK")
            : HealthCheckResult.Unhealthy("RabbitMq connection is ERROR");        
    }
}