using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;


namespace VoteMonitor.Entities
{
    public static class VotingContextExtensions
    {
        private static readonly Dictionary<string, bool> FormsArray = new Dictionary<string, bool>()
        {
            { "A",false},
            { "B",true},
            { "C",true},
            { "D",false},
            { "E",false }
    };

        public static void EnsureSeedData(this VoteMonitorContext context)
        {
            if (!context.AllMigrationsApplied())
            {
                return;
            }

            //means we have data
            //if (context.Counties.Count() > 0)
            //    return;

            using (var tran = context.Database.BeginTransaction())
            {
                context.DataCleanUp(); // why cleanup if we return when we have data? y tho.

                context.SeedNGOs();
                context.SeedCounties();

                context.SeedOptions();
                foreach (var form in FormsArray)
                {
                    context.SeedForms(form.Key, form.Value);
                    context.SeedFormSections(form.Key);
                    context.SeedQuestions(form.Key, form.Value);
                }
                context.SeedObservers();

                tran.Commit();
            }
        }

        private static void SeedObservers(this VoteMonitorContext context)
        {
            if (context.Observers.Any())
            {
                return;
            }

            context.Observers.Add(
                    new Observer() { Id = 0, FromTeam = false, IdNgo = 1, Phone = "0722222222", Name = "Test", Pin = "1234", MobileDeviceId = Guid.NewGuid().ToString(), DeviceRegisterDate = DateTime.Now }
                );

            context.SaveChanges();
        }

        private static void SeedCounties(this VoteMonitorContext context)
        {
            if (context.Counties.Any())
            {
                return;
            }

            context.Counties.AddRange(
                new County { Id = 0, Code = "AB", Name = "ALBA", Diaspora = false },
                new County { Id = 1, Code = "AR", Name = "ARAD", Diaspora = false },
                new County { Id = 2, Code = "AG", Name = "ARGES", Diaspora = false },
                new County { Id = 3, Code = "BC", Name = "BACAU", Diaspora = false },
                new County { Id = 4, Code = "BH", Name = "BIHOR", Diaspora = false },
                new County { Id = 5, Code = "BN", Name = "BISTRITA-NASAUD", Diaspora = false },
                new County { Id = 6, Code = "BT", Name = "BOTOSANI", Diaspora = false },
                new County { Id = 7, Code = "BV", Name = "BRASOV", Diaspora = false },
                new County { Id = 8, Code = "BR", Name = "BRAILA", Diaspora = false },
                new County { Id = 9, Code = "BZ", Name = "BUZAU", Diaspora = false },
                new County { Id = 10, Code = "CS", Name = "CARAS-SEVERIN", Diaspora = false },
                new County { Id = 11, Code = "CL", Name = "CALARASI", Diaspora = false },
                new County { Id = 12, Code = "CJ", Name = "CLUJ", Diaspora = false },
                new County { Id = 13, Code = "CT", Name = "CONSTANTA", Diaspora = false },
                new County { Id = 14, Code = "CV", Name = "COVASNA", Diaspora = false },
                new County { Id = 15, Code = "DB", Name = "DÂMBOVITA", Diaspora = false },
                new County { Id = 16, Code = "DJ", Name = "DOLJ", Diaspora = false },
                new County { Id = 17, Code = "GL", Name = "GALATI", Diaspora = false },
                new County { Id = 18, Code = "GR", Name = "GIURGIU", Diaspora = false },
                new County { Id = 19, Code = "GJ", Name = "GORJ", Diaspora = false },
                new County { Id = 20, Code = "HR", Name = "HARGHITA", Diaspora = false },
                new County { Id = 21, Code = "HD", Name = "HUNEDOARA", Diaspora = false },
                new County { Id = 22, Code = "IL", Name = "IALOMITA", Diaspora = false },
                new County { Id = 23, Code = "IS", Name = "IASI", Diaspora = false },
                new County { Id = 24, Code = "IF", Name = "ILFOV", Diaspora = false },
                new County { Id = 25, Code = "MM", Name = "MARAMURES", Diaspora = false },
                new County { Id = 26, Code = "MH", Name = "MEHEDINTI", Diaspora = false },
                new County { Id = 27, Code = "B", Name = "BUCURESTI", Diaspora = false },
                new County { Id = 28, Code = "MS", Name = "MURES", Diaspora = false },
                new County { Id = 29, Code = "NT", Name = "NEAMT", Diaspora = false },
                new County { Id = 30, Code = "OT", Name = "OLT", Diaspora = false },
                new County { Id = 31, Code = "PH", Name = "PRAHOVA", Diaspora = false },
                new County { Id = 32, Code = "SM", Name = "SATU MARE", Diaspora = false },
                new County { Id = 33, Code = "SJ", Name = "SALAJ", Diaspora = false },
                new County { Id = 34, Code = "SB", Name = "SIBIU", Diaspora = false },
                new County { Id = 35, Code = "SV", Name = "SUCEAVA", Diaspora = false },
                new County { Id = 36, Code = "TR", Name = "TELEORMAN", Diaspora = false },
                new County { Id = 37, Code = "TM", Name = "TIMIS", Diaspora = false },
                new County { Id = 38, Code = "TL", Name = "TULCEA", Diaspora = false },
                new County { Id = 39, Code = "VS", Name = "VASLUI", Diaspora = false },
                new County { Id = 40, Code = "VL", Name = "VÂLCEA", Diaspora = false },
                new County { Id = 41, Code = "VN", Name = "VRANCEA", Diaspora = false },
                new County { Id = 42, Code = "D", Name = "DIASPORA", Diaspora = true }
                );
        }

        private static void DataCleanUp(this VoteMonitorContext context)
        {
            context.Database.ExecuteSqlRaw("delete from NotesAttachments");
            context.Database.ExecuteSqlRaw("delete from OptionsToQuestions");
            context.Database.ExecuteSqlRaw("delete from Questions");
            context.Database.ExecuteSqlRaw("delete from FormSections");
            context.Database.ExecuteSqlRaw("delete from Forms");
            context.Database.ExecuteSqlRaw("delete from Counties");
            context.Database.ExecuteSqlRaw("delete from Observers");
        }

        private static void SeedOptions(this VoteMonitorContext context)
        {
            if (context.Options.Any())
            {
                return;
            }

            context.Options.AddRange(
                new Option { Text = "Da", },
                new Option { Text = "Nu", },
                new Option { Text = "Nu stiu", },
                new Option { Text = "Dark Island", },
                new Option { Text = "London Pride", },
                new Option { Text = "Zaganu", },
                new Option { Text = "Transmisia manualã", },
                new Option { Text = "Transmisia automatã", },
                new Option { Text = "Altele (specificaţi)", IsFreeText = true },
                new Option { Text = "Metrou" },
                new Option { Text = "Tramvai" },
                new Option { Text = "Autobuz" }
            );

            context.SaveChanges();
        }
        private static void SeedFormSections(this VoteMonitorContext context, string formCode)
        {
            var form = context.Forms.SingleOrDefault(f => f.Code == formCode);
            if (form == null)
            {
                return;
            }

            context.FormSections.AddRange(
                new FormSection { Code = formCode + "B", Description = "Despre Bere", IdForm = form.Id },
                new FormSection { Code = formCode + "C", Description = "Description masini", IdForm = form.Id }
                );

            context.SaveChanges();
        }

        private static void SeedQuestions(this VoteMonitorContext context, string formCode, bool diaspora)
        {
            var f = context.Forms.FirstOrDefault(ff => ff.Code == formCode);
            if (f == null)
            {
                f = new Form { Code = formCode, Diaspora = diaspora, Draft = false };
                context.Forms.Add(f);
                context.SaveChanges();
            }

            var fsB = context.FormSections
                .FirstOrDefault(ff => ff.IdForm == f.Id && ff.Code == $"{formCode}B");
            var fsC = context.FormSections
                .FirstOrDefault(ff => ff.IdForm == f.Id && ff.Code == $"{formCode}C");

            if (fsB == null)
            {
                fsB = new FormSection { IdForm = f.Id, Code = $"{formCode}B", Description = $"section B of Form {f.Id}" };
                context.FormSections.Add(fsB);
                context.SaveChanges();
            }
            if (fsC == null)
            {
                fsC = new FormSection { IdForm = f.Id, Code = $"{formCode}C", Description = $"section C of Form {f.Id}" };
                context.FormSections.Add(fsC);
                context.SaveChanges();
            }
            context.Questions.AddRange(
                // primul formular
                new Question
                {
                    IdSection = fsB.Id, //B
                    QuestionType = QuestionType.SingleOption,
                    Text = $"{f.Id}: Iti place berea? (se alege o singura optiune selectabila)",
                    OptionsToQuestions = new List<OptionToQuestion>
                    {
                        new OptionToQuestion {IdOption = 1},
                        new OptionToQuestion {IdOption = 2, Flagged = true},
                        new OptionToQuestion {IdOption = 3}
                    }
                },
                 new Question
                 {
                     IdSection = fsB.Id, //B
                     QuestionType = QuestionType.MultipleOption,
                     Text = $"{f.Id}: Ce tipuri de bere iti plac? (se pot alege optiuni multiple)",
                     OptionsToQuestions = new List<OptionToQuestion>
                    {
                        new OptionToQuestion { IdOption = 4, Flagged = true},
                        new OptionToQuestion { IdOption = 5},
                        new OptionToQuestion { IdOption = 6}
                    }
                 },
                 new Question
                 {
                     IdSection = fsC.Id, //C
                     QuestionType = QuestionType.SingleOptionWithText,
                     Text = $"{f.Id}: Ce tip de transmisie are masina ta? (se poate alege O singura optiune selectabila + text pe O singura optiune)",
                     OptionsToQuestions = new List<OptionToQuestion>
                    {
                        new OptionToQuestion { IdOption = 7, Flagged = true},
                        new OptionToQuestion {IdOption = 8},
                        new OptionToQuestion { IdOption = 9}
                    }
                 },
                 new Question
                 {
                     IdSection = fsC.Id, //C
                     QuestionType = QuestionType.MultipleOptionWithText,
                     Text = $"{f.Id}: Ce mijloace de transport folosesti sa ajungi la birou? (se pot alege mai multe optiuni + text pe O singura optiune)",
                     OptionsToQuestions = new List<OptionToQuestion>
                    {
                        new OptionToQuestion { IdOption = 10, Flagged = true},
                        new OptionToQuestion { IdOption = 11},
                        new OptionToQuestion { IdOption = 12},
                        new OptionToQuestion { IdOption = 9}
                    }
                 }
                );

            context.SaveChanges();

        }

        private static void SeedForms(this VoteMonitorContext context, string formCode, bool diaspora)
        {
            context.Forms.Add(
                 new Form { Code = formCode, Description = "Description " + formCode, CurrentVersion = 1, Diaspora = diaspora, Draft = false }
             );

            context.SaveChanges();
        }

        private static void SeedNGOs(this VoteMonitorContext context)
        {
            if (context.Ngos.Any())
            {
                return;
            }

            context.Ngos.Add(new Ngo
            {
                Id = 1,
                Name = "Code4Romania",
                Organizer = true,
                ShortName = "C4R"
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
