using Spectre.Console;
using VoteMonitor.Entities;

namespace VotingIrregularities.Domain.Seed.Writers
{
    public class NgoAdminsPrinter
    {
        public static void Print(params NgoAdmin[] admins)
        {
            var table = new Table();

            table.AddColumn("Id");
            table.AddColumn("Account");
            table.AddColumn("Ngo Id");
            table.AddColumn("Ngo name");

            foreach (var admin in admins)
            {
                table.AddRow(admin.Id.ToString(), admin.Ngo.Name, admin.Ngo.Id.ToString(), admin.Account);
            }

            AnsiConsole.Write(table);
        }
    }
}
