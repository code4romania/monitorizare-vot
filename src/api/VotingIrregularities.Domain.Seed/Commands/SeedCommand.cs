using Spectre.Console;
using Spectre.Console.Cli;
using System;
using VoteMonitor.Entities;

namespace VotingIrregularities.Domain.Seed.Commands
{
    public class SeedCommand : Command
    {
        private readonly VoteMonitorContext _context;

        public SeedCommand(VoteMonitorContext context)
        {
            _context = context;
        }

        public override int Execute(CommandContext context)
        {
            AnsiConsole.WriteLine("Seeding data migrations");
            try
            {
                Check.AllMigrationsApplied(_context);

                _context.SeedData();
            }
            catch (Exception e) when (e.InnerException != null)
            {
                AnsiConsole.WriteException(e.InnerException);
                return -1;
            }

            AnsiConsole.WriteLine("Done");
            return 0;
        }
    }
}
