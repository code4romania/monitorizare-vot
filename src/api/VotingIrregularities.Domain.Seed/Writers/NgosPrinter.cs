using Spectre.Console;
using VoteMonitor.Entities;

namespace VotingIrregularities.Domain.Seed.Writers
{
    public class NgosPrinter
    {
        public static void Print(params Ngo[] ngos)
        {
            var table = new Table();

            table.AddColumn("Id");
            table.AddColumn("ShortName");
            table.AddColumn("Name");
            table.AddColumn("IsOrganizer");
            table.AddColumn("IsActive");

            foreach (var ngo in ngos)
            {
                table.AddRow(ngo.Id.ToString(), ngo.ShortName, ngo.Name, ngo.Organizer.ToString(), ngo.IsActive.ToString());
            }

            AnsiConsole.Write(table);
        }
    }
}
