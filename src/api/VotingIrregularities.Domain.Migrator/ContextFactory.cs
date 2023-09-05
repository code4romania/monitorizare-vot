using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using VoteMonitor.Entities;

namespace VotingIrregularities.Domain.Migrator;

public class ContextFactory : IDesignTimeDbContextFactory<VoteMonitorContext>
{
    public VoteMonitorContext CreateDbContext(string[] args)
    {
        var configuration = ConfigurationHelper.GetConfiguration();
        var connectionString = configuration.GetConnectionString("DefaultConnection")!;

        var optionsBuilder = new DbContextOptionsBuilder<VoteMonitorContext>();

        optionsBuilder.UseNpgsql(connectionString, x => x.MigrationsAssembly("VotingIrregularities.Domain.Migrator"));

        return new VoteMonitorContext(optionsBuilder.Options);
    }
}
