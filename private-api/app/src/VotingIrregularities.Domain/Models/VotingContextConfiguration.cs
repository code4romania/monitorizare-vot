using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace VotingIrregularities.Domain.Models
{
    /// <summary>
    ///  used only on migrations
    /// </summary>
    public class VotingContextConfiguration : IDbContextFactory<VotingContext>
    {
        public VotingContext Create(DbContextFactoryOptions options)
        {
            var builder = new DbContextOptionsBuilder<VotingContext>();
            builder.UseSqlServer(Startup.RegisterConfiguration().GetConnectionString("DefaultConnection"));
            return new VotingContext(builder.Options);
        }
    }
}
