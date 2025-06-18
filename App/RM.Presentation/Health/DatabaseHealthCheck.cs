using App.Interfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace RM.Presentation.Health;

public class DatabaseHealthCheck : IHealthCheck
{
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly ILogger<DatabaseHealthCheck> _logger;
    public const string NAME = "Database";

    public DatabaseHealthCheck(IDbConnectionFactory dbConnectionFactory, ILogger<DatabaseHealthCheck> logger)
    {
        _dbConnectionFactory = dbConnectionFactory;
        _logger = logger;
    }
    
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new())
    {
        try
        {
            _ = await _dbConnectionFactory.GetConnectionAsync(cancellationToken);
            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            var errorMessage = $"Database is not available: {ex}";
            _logger.LogError(errorMessage);
            return HealthCheckResult.Unhealthy(errorMessage);
        }
    }
}