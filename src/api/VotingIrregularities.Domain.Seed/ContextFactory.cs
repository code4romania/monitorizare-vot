using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using VoteMonitor.Entities;

namespace VotingIrregularities.Domain.Seed
{
    public class ContextFactory : IDesignTimeDbContextFactory<VoteMonitorContext>
    {   
        
        public VoteMonitorContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<VoteMonitorContext>();

            optionsBuilder.UseSqlServer("Server=myServerAddress;Database=myDataBase;Trusted_Connection=True;",
                x => x.MigrationsAssembly("VotingIrregularities.Domain.Seed"));

            return new VoteMonitorContext(optionsBuilder.Options);
        }
    }
}