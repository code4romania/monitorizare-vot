using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace VotingIrregularities.Domain.Models
{
    public partial class VotingContext : DbContext
    {
        public VotingContext(DbContextOptions<VotingContext> options)
            :base(options)
        {
            
        }
    }
}