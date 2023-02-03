using Spectre.Console;
using Spectre.Console.Cli;
using System.Linq;
using System;
using VoteMonitor.Api.HashingService;
using VoteMonitor.Entities;
using VotingIrregularities.Domain.Seed.Writers;

namespace VotingIrregularities.Domain.Seed.Commands
{
    public class AddNgoAdminCommand : Command<AddNgoAdminCommand.Settings>
    {
        private readonly VoteMonitorContext _context;
        private readonly IHashService _hashService;

        public class Settings : CommandSettings
        {
            [CommandArgument(0, "<ngoId>")]
            public int NgoId { get; set; }

            [CommandArgument(1, "<account>")]
            public string Account { get; set; }

            [CommandArgument(2, "<password>")]
            public string Password { get; set; }
        }


        public AddNgoAdminCommand(VoteMonitorContext context, IHashService hashService)
        {
            _context = context;
            _hashService = hashService;
        }

        public override int Execute(CommandContext context, Settings settings)
        {
            try
            {
                Check.AllMigrationsApplied(_context);

                var ngo = _context.Ngos.FirstOrDefault(x => x.Id == settings.NgoId);
                if (ngo == null)
                {
                    AnsiConsole.Write(new Markup($"[red]Could not find requested ngo with id = {settings.NgoId}[/]"));
                    return -1;
                }
                var maxNgoAdminId = 0;

                if (_context.NgoAdmins.Any())
                {
                    maxNgoAdminId = _context.NgoAdmins.Max(x => x.Id);
                }

                var ngoAdmin = new NgoAdmin
                {
                    Id = maxNgoAdminId + 1,
                    Account = settings.Account,
                    Ngo = ngo,
                    Password = _hashService.GetHash(settings.Password)
                };

                _context.NgoAdmins.Add(ngoAdmin);
                _context.SaveChanges();

                AnsiConsole.WriteLine("Created following ngo admin :");
                NgoAdminsPrinter.Print(ngoAdmin);
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
