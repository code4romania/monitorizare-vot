using System;
using VoteMonitor.Entities;

namespace VotingIrregularities.Domain.Seed
{
    public class Check
    {
        public static void AllMigrationsApplied(VoteMonitorContext context)
        {
            if (!context.AllMigrationsApplied())
            {
                throw new Exception("Not all migrations were applied. Please run `apply-migration` command");
            }

        }
    }
}
