using Microsoft.EntityFrameworkCore;
using Spectre.Console;
using Spectre.Console.Cli;
using System;
using System.Linq;
using VoteMonitor.Entities;
using VotingIrregularities.Domain.Seed.Writers;

namespace VotingIrregularities.Domain.Seed.Commands
{
    public class ListNgoCommand : Command
    {
        private readonly VoteMonitorContext _context;

        public ListNgoCommand(VoteMonitorContext context)
        {
            _context = context;
        }
        public override int Execute(CommandContext context)
        {
            try
            {
                Check.AllMigrationsApplied(_context);

                var ngos = _context
                    .Ngos
                    .ToArray();

                if (ngos.Any())
                {
                    NgosPrinter.Print(ngos);
                }
                else
                {
                    AnsiConsole.Write(new Markup("[yellow]There are no ngos[/]"));
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
