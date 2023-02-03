using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using VoteMonitor.Entities;

namespace VotingIrregularities.Domain.Seed
{
    public class VoteMonitorContextFactory : IDesignTimeDbContextFactory<VoteMonitorContext>
    {
        public VoteMonitorContext CreateDbContext(string[] args)
        {
            // Here we create the DbContextOptionsBuilder manually.
            var builder = new DbContextOptionsBuilder<VoteMonitorContext>();

            var connectionString = Settings.Configuration.GetConnectionString("DefaultConnection");
            builder.UseSqlServer(connectionString, b => b.MigrationsAssembly("VotingIrregularities.Domain.Seed"));
            // Create our DbContext.
            return new VoteMonitorContext(builder.Options);
        }
    }
}
