using MassTransit;
using Microsoft.EntityFrameworkCore;
using MassTransit.RetryPolicies;

namespace TrafficCourts.Workflow.Service;

/// <summary>
/// Hosted service to apply database migrations at startup.
/// </summary>
/// <typeparam name="TDbContext"></typeparam>
/// <remarks>Based on <see cref="https://github.com/MassTransit/Sample-Outbox/blob/e4a5aaa1c38768db9f82141529df34c0718f03a5/src/Sample.Api/RecreateDatabaseHostedService.cs"/></remarks>
public class DatabaseMigrationHostedService<TDbContext> :
    IHostedService
    where TDbContext : DbContext
{
    readonly ILogger<DatabaseMigrationHostedService<TDbContext>> _logger;
    readonly IServiceScopeFactory _scopeFactory;
    TDbContext? _context;

    public DatabaseMigrationHostedService(IServiceScopeFactory scopeFactory, ILogger<DatabaseMigrationHostedService<TDbContext>> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Applying migrations for {DbContext}", TypeCache<TDbContext>.ShortName);

        await Retry.Interval(20, TimeSpan.FromSeconds(3)).Retry(async () =>
        {
            var scope = _scopeFactory.CreateScope();

            try
            {
                _context = scope.ServiceProvider.GetRequiredService<TDbContext>();

                await _context.Database.MigrateAsync(cancellationToken);

                _logger.LogInformation("Migrations completed for {DbContext}", TypeCache<TDbContext>.ShortName);
            }
            finally
            {
                if (scope is IAsyncDisposable asyncDisposable)
                    await asyncDisposable.DisposeAsync();
                else
                    scope.Dispose();
            }
        }, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}