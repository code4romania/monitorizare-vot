using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using VoteMonitor.Entities;
using VotingIrregularities.Domain.Seed.Options;
using VotingIrregularities.Domain.Seed.Services;


namespace VotingIrregularities.Domain.Seed
{
    public class VotingContextSeeder
    {
        private readonly VoteMonitorContext _context;
        private readonly IHashService _hashService;
        private readonly SeedOption _options;
        private readonly ILogger<VotingContextSeeder> _logger;

        public VotingContextSeeder(VoteMonitorContext context,
            IHashService hashService,
            SeedOption options,
            ILogger<VotingContextSeeder> logger)
        {
            _context = context;
            _hashService = hashService;
            _options = options;
            _logger = logger;
        }

        public bool SeedData()
        {
            if (!AllMigrationsApplied(_context))
            {
                _logger.LogWarning("Not all migrations were applied please run VotingIrregularities.Domain.Migrator");
                return false;
            }

            if (_context.Ngos.Any()
                || _context.NgoAdmins.Any()
                || _context.Observers.Any()
                || _context.Counties.Any()
                || _context.PollingStations.Any()
                || _context.Forms.Any()
                || _context.FormSections.Any()
                || _context.Questions.Any()
                || _context.Options.Any()
                || _context.OptionsToQuestions.Any()
                || _context.Answers.Any()
                || _context.Notes.Any()
                || _context.NotesAttachments.Any()
                || _context.Notifications.Any()
                || _context.NotificationRecipients.Any())
            {
                if (!_options.OverrideExistingData)
                {
                    _logger.LogWarning("Data exists in DB! If you want to override existing data set OverrideExistingData=true");
                    return false;
                }
            }

            using (var tran = _context.Database.BeginTransaction())
            {
                DataCleanUp();
                SeedCounties();
                SeedForms();
                SeedNGOs();
                SeedPollingStations();

                tran.Commit();
            }

            return true;
        }

        private bool AllMigrationsApplied(DbContext context)
        {
            var applied = context.GetService<IHistoryRepository>()
                .GetAppliedMigrations()
                .Select(m => m.MigrationId);

            var total = context.GetService<IMigrationsAssembly>()
                .Migrations
                .Select(m => m.Key);

            return !total.Except(applied).Any();
        }

        private void DataCleanUp()
        {
            _context.Database.ExecuteSqlRaw(@"delete from public.""NotificationRecipients""");
            _context.Database.ExecuteSqlRaw(@"delete from public.""Notifications""");
            _context.Database.ExecuteSqlRaw(@"delete from public.""NotesAttachments""");
            _context.Database.ExecuteSqlRaw(@"delete from public.""Notes""");
            _context.Database.ExecuteSqlRaw(@"delete from public.""Answers""");
            _context.Database.ExecuteSqlRaw(@"delete from public.""OptionsToQuestions""");
            _context.Database.ExecuteSqlRaw(@"delete from public.""Options""");
            _context.Database.ExecuteSqlRaw(@"delete from public.""Questions""");
            _context.Database.ExecuteSqlRaw(@"delete from public.""FormSections""");
            _context.Database.ExecuteSqlRaw(@"delete from public.""Forms""");
            _context.Database.ExecuteSqlRaw(@"delete from public.""PollingStations""");
            _context.Database.ExecuteSqlRaw(@"delete from public.""Counties""");
            _context.Database.ExecuteSqlRaw(@"delete from public.""Observers""");
            _context.Database.ExecuteSqlRaw(@"delete from public.""NgoAdmin""");
            _context.Database.ExecuteSqlRaw(@"delete from public.""Ngos""");
        }

        private void SeedPollingStations()
        {
            var pollingStations = _context.Counties.ToList().Select(x => new PollingStation()
            {
                Id = x.Id,
                Address = $"{x.Name} main street",
                Coordinates = "22,34",
                IdCounty = x.Id,
                Number = x.Id,
                TerritoryCode = x.Code,
                AdministrativeTerritoryCode = x.Code
            }).ToList();

            _context.PollingStations.AddRange(pollingStations);
            _context.SaveChanges();
        }

        private void SeedCounties()
        {
            _context.Counties.AddRange(
                new County { Id = 0, NumberOfPollingStations = 1, Code = "AB", Name = "ALBA", Diaspora = false },
                new County { Id = 1, NumberOfPollingStations = 1, Code = "AR", Name = "ARAD", Diaspora = false },
                new County { Id = 2, NumberOfPollingStations = 1, Code = "AG", Name = "ARGES", Diaspora = false },
                new County { Id = 3, NumberOfPollingStations = 1, Code = "BC", Name = "BACAU", Diaspora = false },
                new County { Id = 4, NumberOfPollingStations = 1, Code = "BH", Name = "BIHOR", Diaspora = false },
                new County { Id = 5, NumberOfPollingStations = 1, Code = "BN", Name = "BISTRITA-NASAUD", Diaspora = false },
                new County { Id = 6, NumberOfPollingStations = 1, Code = "BT", Name = "BOTOSANI", Diaspora = false },
                new County { Id = 7, NumberOfPollingStations = 1, Code = "BV", Name = "BRASOV", Diaspora = false },
                new County { Id = 8, NumberOfPollingStations = 1, Code = "BR", Name = "BRAILA", Diaspora = false },
                new County { Id = 9, NumberOfPollingStations = 1, Code = "BZ", Name = "BUZAU", Diaspora = false },
                new County { Id = 10, NumberOfPollingStations = 1, Code = "CS", Name = "CARAS-SEVERIN", Diaspora = false },
                new County { Id = 11, NumberOfPollingStations = 1, Code = "CL", Name = "CALARASI", Diaspora = false },
                new County { Id = 12, NumberOfPollingStations = 1, Code = "CJ", Name = "CLUJ", Diaspora = false },
                new County { Id = 13, NumberOfPollingStations = 1, Code = "CT", Name = "CONSTANTA", Diaspora = false },
                new County { Id = 14, NumberOfPollingStations = 1, Code = "CV", Name = "COVASNA", Diaspora = false },
                new County { Id = 15, NumberOfPollingStations = 1, Code = "DB", Name = "DÂMBOVITA", Diaspora = false },
                new County { Id = 16, NumberOfPollingStations = 1, Code = "DJ", Name = "DOLJ", Diaspora = false },
                new County { Id = 17, NumberOfPollingStations = 1, Code = "GL", Name = "GALATI", Diaspora = false },
                new County { Id = 18, NumberOfPollingStations = 1, Code = "GR", Name = "GIURGIU", Diaspora = false },
                new County { Id = 19, NumberOfPollingStations = 1, Code = "GJ", Name = "GORJ", Diaspora = false },
                new County { Id = 20, NumberOfPollingStations = 1, Code = "HR", Name = "HARGHITA", Diaspora = false },
                new County { Id = 21, NumberOfPollingStations = 1, Code = "HD", Name = "HUNEDOARA", Diaspora = false },
                new County { Id = 22, NumberOfPollingStations = 1, Code = "IL", Name = "IALOMITA", Diaspora = false },
                new County { Id = 23, NumberOfPollingStations = 1, Code = "IS", Name = "IASI", Diaspora = false },
                new County { Id = 24, NumberOfPollingStations = 1, Code = "IF", Name = "ILFOV", Diaspora = false },
                new County { Id = 25, NumberOfPollingStations = 1, Code = "MM", Name = "MARAMURES", Diaspora = false },
                new County { Id = 26, NumberOfPollingStations = 1, Code = "MH", Name = "MEHEDINTI", Diaspora = false },
                new County { Id = 27, NumberOfPollingStations = 1, Code = "B", Name = "BUCURESTI", Diaspora = false },
                new County { Id = 28, NumberOfPollingStations = 1, Code = "MS", Name = "MURES", Diaspora = false },
                new County { Id = 29, NumberOfPollingStations = 1, Code = "NT", Name = "NEAMT", Diaspora = false },
                new County { Id = 30, NumberOfPollingStations = 1, Code = "OT", Name = "OLT", Diaspora = false },
                new County { Id = 31, NumberOfPollingStations = 1, Code = "PH", Name = "PRAHOVA", Diaspora = false },
                new County { Id = 32, NumberOfPollingStations = 1, Code = "SM", Name = "SATU MARE", Diaspora = false },
                new County { Id = 33, NumberOfPollingStations = 1, Code = "SJ", Name = "SALAJ", Diaspora = false },
                new County { Id = 34, NumberOfPollingStations = 1, Code = "SB", Name = "SIBIU", Diaspora = false },
                new County { Id = 35, NumberOfPollingStations = 1, Code = "SV", Name = "SUCEAVA", Diaspora = false },
                new County { Id = 36, NumberOfPollingStations = 1, Code = "TR", Name = "TELEORMAN", Diaspora = false },
                new County { Id = 37, NumberOfPollingStations = 1, Code = "TM", Name = "TIMIS", Diaspora = false },
                new County { Id = 38, NumberOfPollingStations = 1, Code = "TL", Name = "TULCEA", Diaspora = false },
                new County { Id = 39, NumberOfPollingStations = 1, Code = "VS", Name = "VASLUI", Diaspora = false },
                new County { Id = 40, NumberOfPollingStations = 1, Code = "VL", Name = "VÂLCEA", Diaspora = false },
                new County { Id = 41, NumberOfPollingStations = 1, Code = "VN", Name = "VRANCEA", Diaspora = false },
                new County { Id = 42, NumberOfPollingStations = 1, Code = "D", Name = "DIASPORA", Diaspora = true }
                );
        }

        private void SeedForms()
        {
            var otherOption = new Option { Text = "Other (please specify)", IsFreeText = true };
            var form1 = new Form
            {
                Code = "A",
                Description = "Description of A",
                CurrentVersion = 1,
                Diaspora = true,
                Draft = false,
                FormSections = new List<FormSection>
                {
                    new FormSection
                    {
                        Code = "A-1",
                        Description = $"From A section 1",
                        OrderNumber = 1,
                        Questions = new List<Question>
                        {
                            new Question
                            {
                                Code =  "A-1-1",
                                Text = "Do you drink beer?",
                                OrderNumber = 1,
                                QuestionType = QuestionType.SingleOption,
                                OptionsToQuestions = new List<OptionToQuestion>
                                {
                                    new OptionToQuestion {Option = new Option { Text = "Yes", IsFreeText = true }},
                                    new OptionToQuestion {Option = new Option { Text = "No", IsFreeText = true }}
                                }
                            },
                            new Question
                            {
                                Code =  "A-1-2",
                                Text = "What is your favorite brand?",
                                OrderNumber = 2,
                                QuestionType = QuestionType.SingleOptionWithText,
                                OptionsToQuestions = new List<OptionToQuestion>
                                {
                                    new OptionToQuestion {Option = new Option { Text = "Carlsberg" }},
                                    new OptionToQuestion {Option = new Option { Text = "Tuborg" }},
                                    new OptionToQuestion {Option = new Option { Text = "Heineken" }, Flagged = true},
                                    new OptionToQuestion {Option = otherOption}
                                }
                            }
                        }
                    },
                    new FormSection
                    {
                        Code = "A-2",
                        Description = "From A section 2",
                        OrderNumber = 2,
                        Questions = new List<Question>
                        {
                            new Question
                            {
                                Code =  "A-2-1",
                                Text = "What social networking sites are you using?",
                                OrderNumber = 1,
                                QuestionType = QuestionType.MultipleOption,
                                OptionsToQuestions = new List<OptionToQuestion>
                                {
                                    new OptionToQuestion {Option = new Option { Text = "Facebook" }},
                                    new OptionToQuestion {Option = new Option { Text = "Twitter" }, Flagged = true},
                                    new OptionToQuestion {Option = new Option { Text = "Linkedin" }},
                                    new OptionToQuestion {Option = new Option { Text = "None" }}
                                }
                            },
                            new Question
                            {
                                Code =  "A-2-2",
                                Text = "What means of transportation do you use?",
                                OrderNumber = 2,
                                QuestionType = QuestionType.MultipleOptionWithText,
                                OptionsToQuestions = new List<OptionToQuestion>
                                {
                                    new OptionToQuestion {Option = new Option { Text = "Metro" }},
                                    new OptionToQuestion {Option = new Option { Text = "Tram" }},
                                    new OptionToQuestion {Option = new Option { Text = "Bus" } },
                                    new OptionToQuestion {Option = otherOption,Flagged = true}
                                }
                            }
                        }
                    }
                }
            };

            var form2 = new Form
            {
                Code = "B",
                Description = "Description of B",
                CurrentVersion = 1,
                Diaspora = true,
                Draft = false,
                FormSections = new List<FormSection>
                {
                    new FormSection
                    {
                        Code = "B-1",
                        Description = "From B section 1",
                        OrderNumber = 1,
                        Questions = new List<Question>
                        {
                            new Question
                            {
                                Code =  "B-1-1",
                                Text = "How many hours do you play video games on average per day?",
                                OrderNumber = 1,
                                QuestionType = QuestionType.SingleOption,
                                OptionsToQuestions = new List<OptionToQuestion>
                                {
                                    new OptionToQuestion {Option = new Option { Text = "I do not play video games" },Flagged = true},
                                    new OptionToQuestion {Option = new Option { Text = "1 hrs" } },
                                    new OptionToQuestion {Option = new Option { Text = "2 hrs" } },
                                    new OptionToQuestion {Option = new Option { Text = "4 hrs" } },
                                    new OptionToQuestion {Option = new Option { Text = "8 hrs" } },
                                    new OptionToQuestion {Option = new Option { Text = "16 hrs" } },
                                    new OptionToQuestion {Option = new Option { Text = "24 hrs" } }
                                }
                            }
                        }
                    },
                    new FormSection
                    {
                        Code = "B-2",
                        Description = "From B section 2",
                        OrderNumber = 2,
                        Questions = new List<Question>
                        {
                            new Question
                            {
                                Code =  "B-2-1",
                                Text = "What is your goto news website?",
                                OrderNumber = 1,
                                QuestionType = QuestionType.SingleOptionWithText,
                                OptionsToQuestions = new List<OptionToQuestion>
                                {
                                    new OptionToQuestion {Option = new Option { Text = "bbc.com" }},
                                    new OptionToQuestion {Option = new Option { Text = "nytimes.com" }},
                                    new OptionToQuestion {Option = new Option { Text = "dailymail.co.uk" }, Flagged = true},
                                    new OptionToQuestion {Option = otherOption}
                                }
                            }
                        }
                    }
                }
            };

            _context.Forms.Add(form1);
            _context.Forms.Add(form2);

            _context.SaveChanges();
        }

        private void SeedNGOs()
        {
            foreach (var ngo in _options.Ngos)
            {
                _context.Ngos.Add(new Ngo
                {
                    Id = int.Parse(ngo.Key),
                    Name = ngo.Value.Name,
                    Organizer = ngo.Value.IsOrganizer,
                    ShortName = ngo.Value.ShortName,
                    IsActive = true,
                    NgoAdmins = ngo.Value.Admins.Select(x => new NgoAdmin()
                    {
                        Id = int.Parse(x.Key),
                        Account = x.Value.Account,
                        Password = _hashService.GetHash(x.Value.Password),

                    }).ToArray(),
                    Observers = ngo.Value.Observers.Select(x => new Observer()
                    {
                        Id = int.Parse(x.Key),
                        Name = x.Value.Name,
                        Phone = x.Value.Phone,
                        Pin = _hashService.GetHash(x.Value.Pin),
                        FromTeam = x.Value.FromTeam
                    }).ToArray()
                });
            }
            _context.SaveChanges();

        }
    }
}
