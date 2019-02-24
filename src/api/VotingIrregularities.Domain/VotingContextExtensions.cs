using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using VotingIrregularities.Domain.Models;
using VotingIrregularities.Domain.ValueObjects;

namespace VotingIrregularities.Domain
{
    public static class VotingContextExtensions
    {
        public static void EnsureSeedData(this VotingContext context)
        {
            if (!context.AllMigrationsApplied())
                return;

            using (var tran = context.Database.BeginTransaction())
            {
                context.DataCleanUp();

                context.SeedNGOs();
                context.SeedVersions();
                context.SeedCounties();
                context.SeedFormSections();
                context.SeedOptions();
                context.SeedQuestions('A');
                context.SeedQuestions('B');
                context.SeedQuestions('C');

                tran.Commit();
            }
        }

        private static void SeedCounties(this VotingContext context)
        {
            if (context.Counties.Any())
                return;

            context.Counties.AddRange(
                new County { Id = 0, Code = "AB", Name = "ALBA" },
                new County { Id = 1, Code = "AR", Name = "ARAD" },
                new County { Id = 2, Code = "AG", Name = "ARGES" },
                new County { Id = 3, Code = "BC", Name = "BACAU" },
                new County { Id = 4, Code = "BH", Name = "BIHOR" },
                new County { Id = 5, Code = "BN", Name = "BISTRITA-NASAUD" },
                new County { Id = 6, Code = "BT", Name = "BOTOSANI" },
                new County { Id = 7, Code = "BV", Name = "BRASOV" },
                new County { Id = 8, Code = "BR", Name = "BRAILA" },
                new County { Id = 9, Code = "BZ", Name = "BUZAU" },
                new County { Id = 10, Code = "CS", Name = "CARAS-SEVERIN" },
                new County { Id = 11, Code = "CL", Name = "CALARASI" },
                new County { Id = 12, Code = "CJ", Name = "CLUJ" },
                new County { Id = 13, Code = "CT", Name = "CONSTANTA" },
                new County { Id = 14, Code = "CV", Name = "COVASNA" },
                new County { Id = 15, Code = "DB", Name = "DÂMBOVITA" },
                new County { Id = 16, Code = "DJ", Name = "DOLJ" },
                new County { Id = 17, Code = "GL", Name = "GALATI" },
                new County { Id = 18, Code = "GR", Name = "GIURGIU" },
                new County { Id = 19, Code = "GJ", Name = "GORJ" },
                new County { Id = 20, Code = "HR", Name = "HARGHITA" },
                new County { Id = 21, Code = "HD", Name = "HUNEDOARA" },
                new County { Id = 22, Code = "IL", Name = "IALOMITA" },
                new County { Id = 23, Code = "IS", Name = "IASI" },
                new County { Id = 24, Code = "IF", Name = "ILFOV" },
                new County { Id = 25, Code = "MM", Name = "MARAMURES" },
                new County { Id = 26, Code = "MH", Name = "MEHEDINTI" },
                new County { Id = 27, Code = "B", Name = "BUCURESTI" },
                new County { Id = 28, Code = "MS", Name = "MURES" },
                new County { Id = 29, Code = "NT", Name = "NEAMT" },
                new County { Id = 30, Code = "OT", Name = "OLT" },
                new County { Id = 31, Code = "PH", Name = "PRAHOVA" },
                new County { Id = 32, Code = "SM", Name = "SATU MARE" },
                new County { Id = 33, Code = "SJ", Name = "SALAJ" },
                new County { Id = 34, Code = "SB", Name = "SIBIU" },
                new County { Id = 35, Code = "SV", Name = "SUCEAVA" },
                new County { Id = 36, Code = "TR", Name = "TELEORMAN" },
                new County { Id = 37, Code = "TM", Name = "TIMIS" },
                new County { Id = 38, Code = "TL", Name = "TULCEA" },
                new County { Id = 39, Code = "VS", Name = "VASLUI" },
                new County { Id = 40, Code = "VL", Name = "VÂLCEA" },
                new County { Id = 41, Code = "VN", Name = "VRANCEA" },
                new County { Id = 42, Code = "D", Name = "DIASPORA" }
                );
        }

        private static void DataCleanUp(this VotingContext context)
        {
            context.Database.ExecuteSqlCommand("delete from OptionsToQuestions");
            context.Database.ExecuteSqlCommand("delete from Questions");
            context.Database.ExecuteSqlCommand("delete from FormSections");
            context.Database.ExecuteSqlCommand("delete from FormVersions");
            context.Database.ExecuteSqlCommand("delete from Counties");
        }

        private static void SeedOptions(this VotingContext context)
        {
            if (context.Options.Any())
                return;
            context.Options.AddRange(
                new Option { Id = 1, Text = "Da", },
                new Option { Id = 2, Text = "Nu", },
                new Option { Id = 3, Text = "Nu stiu", },
                new Option { Id = 4, Text = "Dark Island", },
                new Option { Id = 5, Text = "London Pride", },
                new Option { Id = 6, Text = "Zaganu", },
                new Option { Id = 7, Text = "Transmisia manualã", },
                new Option { Id = 8, Text = "Transmisia automatã", },
                new Option { Id = 9, Text = "Altele (specificaţi)", IsFreeText = true },
                new Option { Id = 10, Text = "Metrou" },
                new Option { Id = 11, Text = "Tramvai" },
                new Option { Id = 12, Text = "Autobuz" }
            );

            context.SaveChanges();
        }
        private static void SeedFormSections(this VotingContext context)
        {
            if (context.FormSections.Any())
                return;

            context.FormSections.AddRange(
                new FormSection { Id = 1, Code = "B", Description = "Despre Bere" },
                new FormSection { Id = 2, Code = "C", Description = "Description masini" }
                );

            context.SaveChanges();
        }

        private static void SeedQuestions(this VotingContext context, char idFormular)
        {
            if (context.Questions.Any(a => a.FormCode == idFormular.ToString()))
                return;

            context.Questions.AddRange(
                // primul formular
                new Question
                {
                    Id = idFormular * 20 + 1,
                    FormCode = idFormular.ToString(),
                    IdSection = 1, //B
                    QuestionType = QuestionType.SingleOption,
                    Text = $"{idFormular}: Iti place berea? (se alege o singura optiune selectabila)",
                    OptionsToQuestions = new List<OptionToQuestion>
                    {
                        new OptionToQuestion {Id = idFormular * 20 + 1, IdOption = 1},
                        new OptionToQuestion {Id = idFormular * 20 + 2, IdOption = 2, Flagged = true},
                        new OptionToQuestion {Id = idFormular * 20 + 3, IdOption = 3}
                    }
                },
                 new Question
                 {
                     Id = idFormular * 20 + 2,
                                    FormCode = idFormular.ToString(),
                                    IdSection = 1, //B
                                    QuestionType = QuestionType.MultipleOption,
                                    Text = $"{idFormular}: Ce tipuri de bere iti plac? (se pot alege optiuni multiple)",
                                    OptionsToQuestions = new List<OptionToQuestion>
                    {
                        new OptionToQuestion {Id = idFormular * 20 + 4, IdOption = 4, Flagged = true},
                        new OptionToQuestion {Id = idFormular * 20 + 5, IdOption = 5},
                        new OptionToQuestion {Id = idFormular * 20 + 6, IdOption = 6}
                    }
                 },
                 new Question
                 {
                     Id = idFormular * 20 + 3,
                     FormCode = idFormular.ToString(),
                     IdSection = 2, //C
                     QuestionType = QuestionType.SingleOptionWithText,
                     Text = $"{idFormular}: Ce tip de transmisie are masina ta? (se poate alege O singura optiune selectabila + text pe O singura optiune)",
                     OptionsToQuestions = new List<OptionToQuestion>
                    {
                        new OptionToQuestion {Id = idFormular * 20 + 7, IdOption = 7, Flagged = true},
                        new OptionToQuestion {Id = idFormular * 20 + 8, IdOption = 8},
                        new OptionToQuestion {Id = idFormular * 20 + 9, IdOption = 9}
                    }
                 },
                 new Question
                 {
                     Id = idFormular * 20 + 4,
                     FormCode = idFormular.ToString(),
                     IdSection = 2, //C
                     QuestionType = QuestionType.MultipleOptionWithText,
                     Text = $"{idFormular}: Ce mijloace de transport folosesti sa ajungi la birou? (se pot alege mai multe optiuni + text pe O singura optiune)",
                     OptionsToQuestions = new List<OptionToQuestion>
                    {
                        new OptionToQuestion {Id = idFormular * 20 + 10, IdOption = 10, Flagged = true},
                        new OptionToQuestion {Id = idFormular * 20 + 11, IdOption = 11},
                        new OptionToQuestion {Id = idFormular * 20 + 12, IdOption = 12},
                        new OptionToQuestion {Id = idFormular * 20 + 13, IdOption = 9}
                    }
                 }
                );

            context.SaveChanges();

        }

        private static void SeedVersions(this VotingContext context)
        {
            if (context.FormVersions.Any())
                return;

            context.FormVersions.AddRange(
                 new FormVersion { Code = "A", CurrentVersion = 1 },
                 new FormVersion { Code = "B", CurrentVersion = 1 },
                 new FormVersion { Code = "C", CurrentVersion = 1 }
             );

            context.SaveChanges();
        }

        private static void SeedNGOs(this VotingContext context)
        {
            if(context.Ngos.Any())
                return;

            context.Ngos.Add(new Ngo
            {
                Id = 1, Name = "Code4Romania", Organizer = true, ShortName = "C4R"
            });
            context.Ngos.Add(new Ngo
            {
                Id = 2,
                Name = "Guest NGO",
                Organizer = false,
                ShortName = "GUE"
            });
            context.SaveChanges();

        }


        private static bool AllMigrationsApplied(this DbContext context)
        {
            var applied = context.GetService<IHistoryRepository>()
                .GetAppliedMigrations()
                .Select(m => m.MigrationId);

            var total = context.GetService<IMigrationsAssembly>()
                .Migrations
                .Select(m => m.Key);

            return !total.Except(applied).Any();
        }
    }
}
