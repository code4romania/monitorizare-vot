using Microsoft.EntityFrameworkCore;
using Spectre.Console;
using Spectre.Console.Cli;
using System;
using System.Linq;
using VoteMonitor.Entities;
using VotingIrregularities.Domain.Seed.Writers;

namespace VotingIrregularities.Domain.Seed.Commands
{
    public class ListNgoAdmins : Command
    {
        private readonly VoteMonitorContext _context;

        public ListNgoAdmins(VoteMonitorContext context)
        {
            _context = context;
        }
        public override int Execute(CommandContext context)
        {
            try
            {
                Check.AllMigrationsApplied(_context);

                var admins = _context
                    .NgoAdmins
                    .Include(x => x.Ngo)
                    .ToArray();

                if (admins.Any())
                {
                    NgoAdminsPrinter.Print(admins);
                }
                else
                {
                    AnsiConsole.Write(new Markup("[yellow]There are no admins[/]"));
                }
            }
            catch (Exception e) when (e.InnerException != null)
            {
                AnsiConsole.WriteException(e.InnerException);
                return -1;
            }

            return 0;
        }
    }
}
