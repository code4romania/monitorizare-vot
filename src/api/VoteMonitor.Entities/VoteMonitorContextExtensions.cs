using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;


namespace VoteMonitor.Entities
{
    public static class VoteMonitorContextExtensions
    {
        private static readonly string[] formsArray = new string[] { "A", "B", "C", "D", "E" };

        public static void EnsureSeedData(this VoteMonitorContext context)
        {
            if (!context.AllMigrationsApplied())
               return;

            //means we have data
            //if (context.Counties.Count() > 0)
            //    return;

            using (var tran = context.Database.BeginTransaction())
            {
                context.DataCleanUp(); // why cleanup if we return when we have data? y tho.

                context.SeedNGOs();
                context.SeedCounties();
                
                context.SeedOptions();
                foreach (var form in formsArray)
                {
                    context.SeedForms(form);
                    context.SeedFormSections(form);
                    context.SeedQuestions(form);
                }
                context.SeedObservers();

                tran.Commit();
            }
        }

        private static void SeedObservers(this VoteMonitorContext context)
        {
            if (context.Observers.Any())
                return;

            context.Observers.Add(
                    new Observer() { Id = 0, FromTeam = false, IdNgo = 1, Phone = "0722222222", Name = "Test", Pin = "1234", MobileDeviceId = Guid.NewGuid().ToString(),DeviceRegisterDate = DateTime.Now }
                );

            context.SaveChanges();
        }

        private static void SeedCounties(this VoteMonitorContext context)
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

        private static void DataCleanUp(this VoteMonitorContext context)
        {
            context.Database.ExecuteSqlCommand("delete from OptionsToQuestions");
            context.Database.ExecuteSqlCommand("delete from Questions");
            context.Database.ExecuteSqlCommand("delete from FormSections");
            context.Database.ExecuteSqlCommand("delete from Forms");
            context.Database.ExecuteSqlCommand("delete from Counties");
            context.Database.ExecuteSqlCommand("delete from Observers");
        }

        private static void SeedOptions(this VoteMonitorContext context)
        {
            if (context.Options.Any())
                return;
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
                return;

            context.FormSections.AddRange(
                new FormSection { Code = formCode + "B", Description = "Despre Bere", IdForm = form.Id },
                new FormSection { Code = formCode + "C", Description = "Description masini", IdForm = form.Id }
                );

            context.SaveChanges();
        }

        private static void SeedQuestions(this VoteMonitorContext context, string formCode)
        {
            var f = context.Forms.FirstOrDefault(ff => ff.Code == formCode);
            if (f == null) {
                f = new Form { Code = formCode };
                context.Forms.Add(f);
                context.SaveChanges();
            }

            var fsB = context.FormSections
                .FirstOrDefault(ff => ff.IdForm == f.Id && ff.Code == $"{formCode}B");
            var fsC = context.FormSections
                .FirstOrDefault(ff => ff.IdForm == f.Id && ff.Code == $"{formCode}C");

            if (fsB== null) {
                fsB = new FormSection{ IdForm = f.Id, Code = $"{formCode}B", Description=$"section B of Form {f.Id}" };
                context.FormSections.Add(fsB);
                context.SaveChanges();
            }
            if (fsC == null) {
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

        private static void SeedForms(this VoteMonitorContext context, string formCode)
        {
            context.Forms.Add(
                 new Form { Code = formCode, Description = "Description " + formCode, CurrentVersion = 1 }
             );

            context.SaveChanges();
        }

        private static void SeedNGOs(this VoteMonitorContext context)
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
