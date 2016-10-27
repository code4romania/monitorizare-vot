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

            using (var tran = context.Database.BeginTransaction())
            {
                context.DataCleanUp();

                context.SeedVersions();
                context.SeedJudete();
                context.SeedSectiune();
                context.SeedOptiuni();
                context.SeedQuestions('A');
                context.SeedQuestions('B');
                context.SeedQuestions('C');

                tran.Commit();
            }
        }

        private static void SeedJudete(this VotingContext context)
        {
            if (context.Judet.Any())
                return;

            context.Judet.AddRange(
                new Judet { IdJudet = 0, CodJudet = "AB", Nume = "ALBA" },
                new Judet { IdJudet = 1, CodJudet = "AR", Nume = "ARAD" },
                new Judet { IdJudet = 2, CodJudet = "AG", Nume = "ARGES" },
                new Judet { IdJudet = 3, CodJudet = "BC", Nume = "BACAU" },
                new Judet { IdJudet = 4, CodJudet = "BH", Nume = "BIHOR" },
                new Judet { IdJudet = 5, CodJudet = "BN", Nume = "BISTRITA-NASAUD" },
                new Judet { IdJudet = 6, CodJudet = "BT", Nume = "BOTOSANI" },
                new Judet { IdJudet = 7, CodJudet = "BV", Nume = "BRASOV" },
                new Judet { IdJudet = 8, CodJudet = "BR", Nume = "BRAILA" },
                new Judet { IdJudet = 9, CodJudet = "BZ", Nume = "BUZAU" },
                new Judet { IdJudet = 10, CodJudet = "CS", Nume = "CARAS-SEVERIN" },
                new Judet { IdJudet = 11, CodJudet = "CL", Nume = "CALARASI" },
                new Judet { IdJudet = 12, CodJudet = "CJ", Nume = "CLUJ" },
                new Judet { IdJudet = 13, CodJudet = "CT", Nume = "CONSTANTA" },
                new Judet { IdJudet = 14, CodJudet = "CV", Nume = "COVASNA" },
                new Judet { IdJudet = 15, CodJudet = "DB", Nume = "DÂMBOVITA" },
                new Judet { IdJudet = 16, CodJudet = "DJ", Nume = "DOLJ" },
                new Judet { IdJudet = 17, CodJudet = "GL", Nume = "GALATI" },
                new Judet { IdJudet = 18, CodJudet = "GR", Nume = "GIURGIU" },
                new Judet { IdJudet = 19, CodJudet = "GJ", Nume = "GORJ" },
                new Judet { IdJudet = 20, CodJudet = "HR", Nume = "HARGHITA" },
                new Judet { IdJudet = 21, CodJudet = "HD", Nume = "HUNEDOARA" },
                new Judet { IdJudet = 22, CodJudet = "IL", Nume = "IALOMITA" },
                new Judet { IdJudet = 23, CodJudet = "IS", Nume = "IASI" },
                new Judet { IdJudet = 24, CodJudet = "IF", Nume = "ILFOV" },
                new Judet { IdJudet = 25, CodJudet = "MM", Nume = "MARAMURES" },
                new Judet { IdJudet = 26, CodJudet = "MH", Nume = "MEHEDINTI" },
                new Judet { IdJudet = 27, CodJudet = "B", Nume = "MUNICIPIUL BUCURESTI" },
                new Judet { IdJudet = 28, CodJudet = "MS", Nume = "MURES" },
                new Judet { IdJudet = 29, CodJudet = "NT", Nume = "NEAMT" },
                new Judet { IdJudet = 30, CodJudet = "OT", Nume = "OLT" },
                new Judet { IdJudet = 31, CodJudet = "PH", Nume = "PRAHOVA" },
                new Judet { IdJudet = 32, CodJudet = "SM", Nume = "SATU MARE" },
                new Judet { IdJudet = 33, CodJudet = "SJ", Nume = "SALAJ" },
                new Judet { IdJudet = 34, CodJudet = "SB", Nume = "SIBIU" },
                new Judet { IdJudet = 35, CodJudet = "SV", Nume = "SUCEAVA" },
                new Judet { IdJudet = 36, CodJudet = "TR", Nume = "TELEORMAN" },
                new Judet { IdJudet = 37, CodJudet = "TM", Nume = "TIMIS" },
                new Judet { IdJudet = 38, CodJudet = "TL", Nume = "TULCEA" },
                new Judet { IdJudet = 39, CodJudet = "VS", Nume = "VASLUI" },
                new Judet { IdJudet = 40, CodJudet = "VL", Nume = "VÂLCEA" },
                new Judet { IdJudet = 41, CodJudet = "VN", Nume = "VRANCEA" }
                );
        }

        private static void DataCleanUp(this VotingContext context)
        {
            context.Database.ExecuteSqlCommand("delete from RaspunsDisponibil");
            context.Database.ExecuteSqlCommand("delete from Intrebare");
            context.Database.ExecuteSqlCommand("delete from Sectiune");
            context.Database.ExecuteSqlCommand("delete from VersiuneFormular");
           // context.Database.ExecuteSqlCommand("delete from Judet");
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

        private static void SeedQuestions(this VotingContext context, char idFormular)
        {
            if (context.Intrebare.Any(a => a.CodFormular == idFormular.ToString()))
                return;

            context.Intrebare.AddRange(
                // primul formular
                new Intrebare
                {
                    IdIntrebare = idFormular * 20 + 1,
                    CodFormular = idFormular.ToString(),
                    IdSectiune = 1, //B
                    IdTipIntrebare = TipIntrebareEnum.OSinguraOptiune,
                    TextIntrebare = $"{idFormular}: Iti place berea? (se alege o singura optiune selectabila)",
                    RaspunsDisponibil = new List<RaspunsDisponibil>
                    {
                        new RaspunsDisponibil {IdRaspunsDisponibil = idFormular * 20 + 1, IdOptiune = 1},
                        new RaspunsDisponibil {IdRaspunsDisponibil = idFormular * 20 + 2, IdOptiune = 2, RaspunsCuFlag = true},
                        new RaspunsDisponibil {IdRaspunsDisponibil = idFormular * 20 + 3, IdOptiune = 3}
                    }
                },
                 new Intrebare
                 {
                     IdIntrebare = idFormular * 20 + 2,
                                    CodFormular = idFormular.ToString(),
                                    IdSectiune = 1, //B
                                    IdTipIntrebare = TipIntrebareEnum.OptiuniMultiple,
                                    TextIntrebare = $"{idFormular}: Ce tipuri de bere iti plac? (se pot alege optiuni multiple)",
                                    RaspunsDisponibil = new List<RaspunsDisponibil>
                    {
                        new RaspunsDisponibil {IdRaspunsDisponibil = idFormular * 20 + 4, IdOptiune = 4, RaspunsCuFlag = true},
                        new RaspunsDisponibil {IdRaspunsDisponibil = idFormular * 20 + 5, IdOptiune = 5},
                        new RaspunsDisponibil {IdRaspunsDisponibil = idFormular * 20 + 6, IdOptiune = 6}
                    }
                 },
                 new Intrebare
                 {
                     IdIntrebare = idFormular * 20 + 3,
                     CodFormular = idFormular.ToString(),
                     IdSectiune = 2, //C
                     IdTipIntrebare = TipIntrebareEnum.OSinguraOptiuneCuText,
                     TextIntrebare = $"{idFormular}: Ce tip de transmisie are masina ta? (se poate alege O singura optiune selectabila + text pe O singura optiune)",
                     RaspunsDisponibil = new List<RaspunsDisponibil>
                    {
                        new RaspunsDisponibil {IdRaspunsDisponibil = idFormular * 20 + 7, IdOptiune = 7, RaspunsCuFlag = true},
                        new RaspunsDisponibil {IdRaspunsDisponibil = idFormular * 20 + 8, IdOptiune = 8},
                        new RaspunsDisponibil {IdRaspunsDisponibil = idFormular * 20 + 9, IdOptiune = 9}
                    }
                 },
                 new Intrebare
                 {
                     IdIntrebare = idFormular * 20 + 4,
                     CodFormular = idFormular.ToString(),
                     IdSectiune = 2, //C
                     IdTipIntrebare = TipIntrebareEnum.OptiuniMultipleCuText,
                     TextIntrebare = $"{idFormular}: Ce mijloace de transport folosesti sa ajungi la birou? (se pot alege mai multe optiuni + text pe O singura optiune)",
                     RaspunsDisponibil = new List<RaspunsDisponibil>
                    {
                        new RaspunsDisponibil {IdRaspunsDisponibil = idFormular * 20 + 10, IdOptiune = 10, RaspunsCuFlag = true},
                        new RaspunsDisponibil {IdRaspunsDisponibil = idFormular * 20 + 11, IdOptiune = 11},
                        new RaspunsDisponibil {IdRaspunsDisponibil = idFormular * 20 + 12, IdOptiune = 12},
                        new RaspunsDisponibil {IdRaspunsDisponibil = idFormular * 20 + 13, IdOptiune = 9}
                    }
                 }
                );

            context.SaveChanges();

        }

        private static void SeedVersions(this VotingContext context)
        {
            if (context.VersiuneFormular.Any())
                return;

            context.VersiuneFormular.AddRange(
                 new VersiuneFormular { CodFormular = "A", VersiuneaCurenta = 1 },
                 new VersiuneFormular { CodFormular = "B", VersiuneaCurenta = 1 },
                 new VersiuneFormular { CodFormular = "C", VersiuneaCurenta = 1 }
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
