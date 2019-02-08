using System;
using Microsoft.EntityFrameworkCore.Design;

namespace VotingIrregularities.Domain.Models
{
    /// <summary>
    ///  used only on migrations
    /// </summary>
    public class VotingContextConfiguration : IDesignTimeDbContextFactory<VotingContext>
    {
        //public VotingContext Create(DbContextFactoryOptions options)
        //{
        //    var builder = new DbContextOptionsBuilder<VotingContext>();
        //    builder.UseSqlServer(Startup.RegisterConfiguration().GetConnectionString("DefaultConnection"));
        //    return new VotingContext(builder.Options);
        //}

        public VotingContext CreateDbContext(string[] args)
        {
            throw new NotImplementedException();
        }
    }
}
