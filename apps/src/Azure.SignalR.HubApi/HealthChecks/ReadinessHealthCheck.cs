using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Azure.SignalR.HubApi.HealthChecks;

internal class ReadinessHealthCheck : IHealthCheck
{
    private volatile bool _isReady;
    
    public bool StartupCompleted
    {
        get => _isReady;
        set => _isReady = value;
    }
    
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, 
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(StartupCompleted
            ? HealthCheckResult.Healthy("The startup task has completed.")
            : HealthCheckResult.Unhealthy("That startup task is still running.")
        );
    }
}

internal class ReadinessBackgroundService : BackgroundService
{
    private readonly ReadinessHealthCheck _healthCheck;

    public ReadinessBackgroundService(ReadinessHealthCheck healthCheck)
    {
        _healthCheck = healthCheck;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _healthCheck.StartupCompleted = true;
        
        return Task.CompletedTask;
    }
}