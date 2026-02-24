using Microsoft.Extensions.Diagnostics.HealthChecks;
using RabbitMQ.Client;

namespace ManagerTask.HealthChecks;

public class RabbitMqHealthCheck : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        try
        {
            var factory = new ConnectionFactory(){HostName = "localhost"};
            await using var connection = await factory.CreateConnectionAsync(cancellationToken);
            return connection.IsOpen ? HealthCheckResult.Healthy("RabbitMq connection is OK")
                : HealthCheckResult.Unhealthy("RabbitMq connection is ERROR");
        }
        catch (Exception e)
        {
            return HealthCheckResult.Unhealthy(e.ToString());
        }
    }
}