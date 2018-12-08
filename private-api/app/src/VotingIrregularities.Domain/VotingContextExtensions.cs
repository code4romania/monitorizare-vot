using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using VotingIrregularities.Domain.Models;
using VotingIrregularities.Domain.ValueObjects;

namespace VotingIrregularities.Domain.Migrations
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

                context.SeedVersions();
                context.SeedCountye();
                context.SeedSectiune();
                context.SeedOptiuni();
                context.SeedQuestions('A');
                context.SeedQuestions('B');
                context.SeedQuestions('C');

                tran.Commit();
            }
        }

        private static void SeedCountye(this VotingContext context)
        {
            if (context.Counties.Any())
                return;

            context.Counties.AddRange(
                new County { CountyId = 1, CountyCode = "AB", Name = "ALBA" },
                new County { CountyId = 2, CountyCode = "AR", Name = "ARAD" },
                new County { CountyId = 3, CountyCode = "AG", Name = "ARGES" },
                new County { CountyId = 4, CountyCode = "BC", Name = "BACAU" },
                new County { CountyId = 5, CountyCode = "BH", Name = "BIHOR" },
                new County { CountyId = 6, CountyCode = "BN", Name = "BISTRITA-NASAUD" },
                new County { CountyId = 7, CountyCode = "BT", Name = "BOTOSANI" },
                new County { CountyId = 8, CountyCode = "BV", Name = "BRASOV" },
                new County { CountyId = 9, CountyCode = "BR", Name = "BRAILA" },
                new County { CountyId = 10, CountyCode = "BZ", Name = "BUZAU" },
                new County { CountyId = 11, CountyCode = "CS", Name = "CARAS-SEVERIN" },
                new County { CountyId = 12, CountyCode = "CL", Name = "CALARASI" },
                new County { CountyId = 13, CountyCode = "CJ", Name = "CLUJ" },
                new County { CountyId = 14, CountyCode = "CT", Name = "CONSTANTA" },
                new County { CountyId = 15, CountyCode = "CV", Name = "COVASNA" },
                new County { CountyId = 16, CountyCode = "DB", Name = "DÂMBOVITA" },
                new County { CountyId = 17, CountyCode = "DJ", Name = "DOLJ" },
                new County { CountyId = 18, CountyCode = "GL", Name = "GALATI" },
                new County { CountyId = 19, CountyCode = "GR", Name = "GIURGIU" },
                new County { CountyId = 20, CountyCode = "GJ", Name = "GORJ" },
                new County { CountyId = 21, CountyCode = "HR", Name = "HARGHITA" },
                new County { CountyId = 22, CountyCode = "HD", Name = "HUNEDOARA" },
                new County { CountyId = 23, CountyCode = "IL", Name = "IALOMITA" },
                new County { CountyId = 24, CountyCode = "IS", Name = "IASI" },
                new County { CountyId = 25, CountyCode = "IF", Name = "ILFOV" },
                new County { CountyId = 26, CountyCode = "MM", Name = "MARAMURES" },
                new County { CountyId = 27, CountyCode = "MH", Name = "MEHEDINTI" },
                new County { CountyId = 28, CountyCode = "B", Name = "MUNICIPIUL BUCURESTI" },
                new County { CountyId = 29, CountyCode = "MS", Name = "MURES" },
                new County { CountyId = 30, CountyCode = "NT", Name = "NEAMT" },
                new County { CountyId = 31, CountyCode = "OT", Name = "OLT" },
                new County { CountyId = 32, CountyCode = "PH", Name = "PRAHOVA" },
                new County { CountyId = 33, CountyCode = "SM", Name = "SATU MARE" },
                new County { CountyId = 34, CountyCode = "SJ", Name = "SALAJ" },
                new County { CountyId = 35, CountyCode = "SB", Name = "SIBIU" },
                new County { CountyId = 36, CountyCode = "SV", Name = "SUCEAVA" },
                new County { CountyId = 37, CountyCode = "TR", Name = "TELEORMAN" },
                new County { CountyId = 38, CountyCode = "TM", Name = "TIMIS" },
                new County { CountyId = 39, CountyCode = "TL", Name = "TULCEA" },
                new County { CountyId = 40, CountyCode = "VS", Name = "VASLUI" },
                new County { CountyId = 41, CountyCode = "VL", Name = "VÂLCEA" },
                new County { CountyId = 42, CountyCode = "VN", Name = "VRANCEA" },
                new County { CountyId = 43, CountyCode = "D", Name = "DIASPORA" }
                );
        }

        private static void DataCleanUp(this VotingContext context)
        {
            context.Database.ExecuteSqlCommand("delete from AvailableAnswer");
            context.Database.ExecuteSqlCommand("delete from Question");
            context.Database.ExecuteSqlCommand("delete from Sectiune");
            context.Database.ExecuteSqlCommand("delete from FormVersion");
           // context.Database.ExecuteSqlCommand("delete from County");
        }

        private static void SeedOptiuni(this VotingContext context)
        {
            if (context.Options.Any())
                return;
            context.Options.AddRange(
                new Option { OptionId = 1, TextOption = "Da", },
                new Option { OptionId = 2, TextOption = "Nu", },
                new Option { OptionId = 3, TextOption = "Nu stiu", },
                new Option { OptionId = 4, TextOption = "Dark Island", },
                new Option { OptionId = 5, TextOption = "London Pride", },
                new Option { OptionId = 6, TextOption = "Zaganu", },
                new Option { OptionId = 7, TextOption = "Transmisia manualã", },
                new Option { OptionId = 8, TextOption = "Transmisia automatã", },
                new Option { OptionId = 9, TextOption = "Altele (specificaţi)", TextMustBeInserted = true },
                new Option { OptionId = 10, TextOption = "Metrou" },
                new Option { OptionId = 11, TextOption = "Tramvai" },
                new Option { OptionId = 12, TextOption = "Autobuz" }
            );

            context.SaveChanges();
        }
        private static void SeedSectiune(this VotingContext context)
        {
            if (context.Sections.Any())
                return;

            context.Sections.AddRange(
                new Section { SectionId = 1, SectionCode = "B", Description = "Despre Bere" },
                new Section { SectionId = 2, SectionCode = "C", Description = "Description masini" }
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
                    QuestionId = idFormular * 20 + 1,
                    FormCode = idFormular.ToString(),
                    SectionId = 1, //B
                    QuestionTypeId = QuestionTypeEnum.SingleOption,
                    QuestionText = $"{idFormular}: Iti place berea? (se alege o singura Option selectabila)",
                    AvailableAnswer = new List<AvailableAnswer>
                    {
                        new AvailableAnswer {AvailableAnswerId = idFormular * 20 + 1, OptionId = 1},
                        new AvailableAnswer {AvailableAnswerId = idFormular * 20 + 2, OptionId = 2, AnswerWithFlag = true},
                        new AvailableAnswer {AvailableAnswerId = idFormular * 20 + 3, OptionId = 3}
                    }
                },
                 new Question
                 {
                     QuestionId = idFormular * 20 + 2,
                                    FormCode = idFormular.ToString(),
                                    SectionId = 1, //B
                                    QuestionTypeId = QuestionTypeEnum.MultipleOptions,
                                    QuestionText = $"{idFormular}: Ce tipuri de bere iti plac? (se pot alege optiuni multiple)",
                                    AvailableAnswer = new List<AvailableAnswer>
                    {
                        new AvailableAnswer {AvailableAnswerId = idFormular * 20 + 4, OptionId = 4, AnswerWithFlag = true},
                        new AvailableAnswer {AvailableAnswerId = idFormular * 20 + 5, OptionId = 5},
                        new AvailableAnswer {AvailableAnswerId = idFormular * 20 + 6, OptionId = 6}
                    }
                 },
                 new Question
                 {
                     QuestionId = idFormular * 20 + 3,
                     FormCode = idFormular.ToString(),
                     SectionId = 2, //C
                     QuestionTypeId = QuestionTypeEnum.SingleOptionWithText,
                     QuestionText = $"{idFormular}: Ce tip de transmisie are masina ta? (se poate alege O singura Option selectabila + text pe O singura Option)",
                     AvailableAnswer = new List<AvailableAnswer>
                    {
                        new AvailableAnswer {AvailableAnswerId = idFormular * 20 + 7, OptionId = 7, AnswerWithFlag = true},
                        new AvailableAnswer {AvailableAnswerId = idFormular * 20 + 8, OptionId = 8},
                        new AvailableAnswer {AvailableAnswerId = idFormular * 20 + 9, OptionId = 9}
                    }
                 },
                 new Question
                 {
                     QuestionId = idFormular * 20 + 4,
                     FormCode = idFormular.ToString(),
                     SectionId = 2, //C
                     QuestionTypeId = QuestionTypeEnum.MultipleOptionsWithText,
                     QuestionText = $"{idFormular}: Ce mijloace de transport folosesti sa ajungi la birou? (se pot alege mai multe optiuni + text pe O singura Option)",
                     AvailableAnswer = new List<AvailableAnswer>
                    {
                        new AvailableAnswer {AvailableAnswerId = idFormular * 20 + 10, OptionId = 10, AnswerWithFlag = true},
                        new AvailableAnswer {AvailableAnswerId = idFormular * 20 + 11, OptionId = 11},
                        new AvailableAnswer {AvailableAnswerId = idFormular * 20 + 12, OptionId = 12},
                        new AvailableAnswer {AvailableAnswerId = idFormular * 20 + 13, OptionId = 9}
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
                 new FormVersion { FormCode = "A", CurrentVersion = 1 },
                 new FormVersion { FormCode = "B", CurrentVersion = 1 },
                 new FormVersion { FormCode = "C", CurrentVersion = 1 }
             );

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
