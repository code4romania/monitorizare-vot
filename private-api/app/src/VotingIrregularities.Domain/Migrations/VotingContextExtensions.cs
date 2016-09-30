using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

            context.SeedVersions();
            context.CleanupFormulare();
            context.SeedSectiune();
            context.SeedOptiuni();
            context.SeedQuestions();
        }

        private static void CleanupFormulare(this VotingContext context)
        {
            context.Database.ExecuteSqlCommand("delete from RaspunsDisponibil");
            context.Database.ExecuteSqlCommand("delete from Intrebare");
            context.Database.ExecuteSqlCommand("delete from Sectiune");
        }

        private static void SeedOptiuni(this VotingContext context)
        {
            if (context.Optiune.Any())
                return;
            context.Optiune.AddRange(
                new Optiune { IdOptiune = 1, TextOptiune = "Da", },
                new Optiune { IdOptiune = 2, TextOptiune = "Nu", },
                new Optiune { IdOptiune = 3, TextOptiune = "Nu stiu", },
                new Optiune { IdOptiune = 4, TextOptiune = "Dark Island", },
                new Optiune { IdOptiune = 5, TextOptiune = "London Pride", },
                new Optiune { IdOptiune = 6, TextOptiune = "Zaganu", },
                new Optiune { IdOptiune = 7, TextOptiune = "Transmisia manualã", },
                new Optiune { IdOptiune = 8, TextOptiune = "Transmisia automatã", },
                new Optiune { IdOptiune = 9, TextOptiune = "Altele (specificaţi)", SeIntroduceText = true },
                new Optiune { IdOptiune = 10, TextOptiune = "Metrou" },
                new Optiune { IdOptiune = 11, TextOptiune = "Tramvai" },
                new Optiune { IdOptiune = 12, TextOptiune = "Autobuz" }
            );

            context.SaveChanges();
        }
        private static void SeedSectiune(this VotingContext context)
        {
            if (context.Sectiune.Any())
                return;

            context.Sectiune.AddRange(
                new Sectiune { IdSectiune = 1, CodSectiune = "B", Descriere = "Despre Bere" },
                new Sectiune { IdSectiune = 2, CodSectiune = "C", Descriere = "Descriere masini" }
                );

            context.SaveChanges();
        }

        private static void SeedQuestions(this VotingContext context)
        {
            if (context.Intrebare.Any())
                return;

            context.Intrebare.AddRange(
                // primul formular
                new Intrebare
                {
                    IdIntrebare = 1,
                    CodFormular = "A",
                    IdSectiune = 1, //B
                    IdTipIntrebare = TipIntrebareEnum.OSinguraOptiune,
                    TextIntrebare = "Iti place berea? (se alege o singura optiune selectabila)",
                    RaspunsDisponibil = new List<RaspunsDisponibil>
                    {
                        new RaspunsDisponibil {IdRaspunsDisponibil = 1, IdOptiune = 1},
                        new RaspunsDisponibil {IdRaspunsDisponibil = 2, IdOptiune = 2, RaspunsCuFlag = true},
                        new RaspunsDisponibil {IdRaspunsDisponibil = 3, IdOptiune = 3}
                    }
                },
                 new Intrebare
                 {
                     IdIntrebare = 2,
                                    CodFormular = "A",
                                    IdSectiune = 1, //B
                                    IdTipIntrebare = TipIntrebareEnum.OptiuniMultiple,
                                    TextIntrebare = "Ce tipuri de bere iti plac? (se pot alege optiuni multiple)",
                                    RaspunsDisponibil = new List<RaspunsDisponibil>
                    {
                        new RaspunsDisponibil {IdRaspunsDisponibil = 4, IdOptiune = 4, RaspunsCuFlag = true},
                        new RaspunsDisponibil {IdRaspunsDisponibil = 5, IdOptiune = 5},
                        new RaspunsDisponibil {IdRaspunsDisponibil = 6, IdOptiune = 6}
                    }
                 },
                 new Intrebare
                 {
                     IdIntrebare = 3,
                     CodFormular = "A",
                     IdSectiune = 2, //C
                     IdTipIntrebare = TipIntrebareEnum.OSinguraOptiuneCuText,
                     TextIntrebare = "Ce tip de transmisie are masina ta? (se poate alege O singura optiune selectabila + text pe O singura optiune)",
                     RaspunsDisponibil = new List<RaspunsDisponibil>
                    {
                        new RaspunsDisponibil {IdRaspunsDisponibil = 7, IdOptiune = 7, RaspunsCuFlag = true},
                        new RaspunsDisponibil {IdRaspunsDisponibil = 8, IdOptiune = 8},
                        new RaspunsDisponibil {IdRaspunsDisponibil = 9, IdOptiune = 9}
                    }
                 },
                 new Intrebare
                 {
                     IdIntrebare = 4,
                     CodFormular = "A",
                     IdSectiune = 2, //C
                     IdTipIntrebare = TipIntrebareEnum.OptiuniMultipleCuText,
                     TextIntrebare = "Ce mijloace de transport folosesti sa ajungi la birou? (se pot alege mai multe optiuni + text pe O singura optiune)",
                     RaspunsDisponibil = new List<RaspunsDisponibil>
                    {
                        new RaspunsDisponibil {IdRaspunsDisponibil = 10, IdOptiune = 10, RaspunsCuFlag = true},
                        new RaspunsDisponibil {IdRaspunsDisponibil = 11, IdOptiune = 11},
                        new RaspunsDisponibil {IdRaspunsDisponibil = 12, IdOptiune = 12},
                        new RaspunsDisponibil {IdRaspunsDisponibil = 13, IdOptiune = 9}
                    }
                 }
                );

            context.SaveChanges();

        }

        private static void SeedVersions(this VotingContext context)
        {
            if (context.VersiuneFormular.Any())
            {
                context.Database.ExecuteSqlCommand("delete from VersiuneFormular");
                return;
            }

            context.VersiuneFormular.AddRange(
                 new VersiuneFormular { CodFormular = "A", VersiuneaCurenta = 1 },
                 new VersiuneFormular { CodFormular = "B", VersiuneaCurenta = 2 },
                 new VersiuneFormular { CodFormular = "C", VersiuneaCurenta = 3 }
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
