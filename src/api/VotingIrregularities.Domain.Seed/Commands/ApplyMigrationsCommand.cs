using Microsoft.EntityFrameworkCore;
using Spectre.Console;
using Spectre.Console.Cli;
using System;
using VoteMonitor.Entities;

namespace VotingIrregularities.Domain.Seed.Commands
{
    public class ApplyMigrationsCommand : Command
    {
        private readonly VoteMonitorContext _context;

        public ApplyMigrationsCommand(VoteMonitorContext context)
        {
            _context = context;
        }

        public override int Execute(CommandContext context)
        {
            AnsiConsole.WriteLine("Applying migrations");
            try
            {
                _context.Database.Migrate();
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
