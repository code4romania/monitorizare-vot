using Microsoft.EntityFrameworkCore;

namespace VoteMonitor.Entities
{
    public partial class VoteMonitorContext : DbContext
    {
        public VoteMonitorContext(DbContextOptions<VoteMonitorContext> options)
            :base(options)
        {

        }
    }
}