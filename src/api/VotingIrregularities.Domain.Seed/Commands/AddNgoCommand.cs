using Spectre.Console;
using Spectre.Console.Cli;
using System.Linq;
using System;
using VoteMonitor.Entities;
using VotingIrregularities.Domain.Seed.Writers;

namespace VotingIrregularities.Domain.Seed.Commands
{
    public class AddNgoCommand : Command<AddNgoCommand.Settings>
    {
        private readonly VoteMonitorContext _context;

        public class Settings : CommandSettings
        {
            [CommandArgument(0, "<shortName>")]
            public string ShortName { get; set; }

            [CommandArgument(1, "<name>")]
            public string Name { get; set; }

            [CommandArgument(2, "<isOrganizer>")]
            public bool IsOrganizer { get; set; }

            [CommandArgument(3, "<isActive>")]
            public bool IsActive { get; set; }
        }

        public AddNgoCommand(VoteMonitorContext context)
        {
            _context = context;
        }

        public override int Execute(CommandContext context, Settings settings)
        {
            try
            {
                Check.AllMigrationsApplied(_context);

                int maxNgoId = 0;
                if (_context.Ngos.Any())
                {
                    maxNgoId = _context.Ngos.Max(x => x.Id);
                }

                var newNgo = new Ngo
                {
                    Id = maxNgoId + 1,
                    ShortName = settings.ShortName,
                    Name = settings.Name,
                    IsActive = settings.IsActive,
                    Organizer = settings.IsOrganizer
                };

                _context.Ngos.Add(newNgo);
                _context.SaveChanges();

                AnsiConsole.WriteLine("Created following ngo:");
                NgosPrinter.Print(newNgo);
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
