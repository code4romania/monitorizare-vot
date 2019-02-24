using Microsoft.EntityFrameworkCore;

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