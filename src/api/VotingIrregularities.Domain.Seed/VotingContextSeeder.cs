using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Logging;
using VoteMonitor.Entities;
using VotingIrregularities.Domain.Seed.Options;
using VotingIrregularities.Domain.Seed.Services;


namespace VotingIrregularities.Domain.Seed;

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
            || _context.Provinces.Any()
            || _context.Counties.Any()
            || _context.Municipalities.Any()
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
            SeedForms();
            SeedNGOs();

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
        _context.Database.ExecuteSqlRaw(@"delete from public.""PollingStationInfos""");
        _context.Database.ExecuteSqlRaw(@"delete from public.""PollingStations""");
        _context.Database.ExecuteSqlRaw(@"delete from public.""Provinces""");
        _context.Database.ExecuteSqlRaw(@"delete from public.""Counties""");
        _context.Database.ExecuteSqlRaw(@"delete from public.""Municipalities""");
        _context.Database.ExecuteSqlRaw(@"delete from public.""Observers""");
        _context.Database.ExecuteSqlRaw(@"delete from public.""NgoAdmin""");
        _context.Database.ExecuteSqlRaw(@"delete from public.""Ngos""");
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
                new()
                {
                    Code = "A-1",
                    Description = $"From A section 1",
                    OrderNumber = 1,
                    Questions = new List<Question>
                    {
                        new()
                        {
                            Code =  "A-1-1",
                            Text = "Do you drink beer?",
                            OrderNumber = 1,
                            QuestionType = QuestionType.SingleOption,
                            OptionsToQuestions = new List<OptionToQuestion>
                            {
                                new() {Option = new Option { Text = "Yes", IsFreeText = true }},
                                new() {Option = new Option { Text = "No", IsFreeText = true }}
                            }
                        },
                        new()
                        {
                            Code =  "A-1-2",
                            Text = "What is your favorite brand?",
                            OrderNumber = 2,
                            QuestionType = QuestionType.SingleOptionWithText,
                            OptionsToQuestions = new List<OptionToQuestion>
                            {
                                new() {Option = new Option { Text = "Carlsberg" }},
                                new() {Option = new Option { Text = "Tuborg" }},
                                new() {Option = new Option { Text = "Heineken" }, Flagged = true},
                                new() {Option = otherOption}
                            }
                        }
                    }
                },
                new()
                {
                    Code = "A-2",
                    Description = "From A section 2",
                    OrderNumber = 2,
                    Questions = new List<Question>
                    {
                        new()
                        {
                            Code =  "A-2-1",
                            Text = "What social networking sites are you using?",
                            OrderNumber = 1,
                            QuestionType = QuestionType.MultipleOption,
                            OptionsToQuestions = new List<OptionToQuestion>
                            {
                                new() {Option = new Option { Text = "Facebook" }},
                                new() {Option = new Option { Text = "Twitter" }, Flagged = true},
                                new() {Option = new Option { Text = "Linkedin" }},
                                new() {Option = new Option { Text = "None" }}
                            }
                        },
                        new()
                        {
                            Code =  "A-2-2",
                            Text = "What means of transportation do you use?",
                            OrderNumber = 2,
                            QuestionType = QuestionType.MultipleOptionWithText,
                            OptionsToQuestions = new List<OptionToQuestion>
                            {
                                new() {Option = new Option { Text = "Metro" }},
                                new() {Option = new Option { Text = "Tram" }},
                                new() {Option = new Option { Text = "Bus" } },
                                new() {Option = otherOption,Flagged = true}
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
                new()
                {
                    Code = "B-1",
                    Description = "From B section 1",
                    OrderNumber = 1,
                    Questions = new List<Question>
                    {
                        new()
                        {
                            Code =  "B-1-1",
                            Text = "How many hours do you play video games on average per day?",
                            OrderNumber = 1,
                            QuestionType = QuestionType.SingleOption,
                            OptionsToQuestions = new List<OptionToQuestion>
                            {
                                new() {Option = new Option { Text = "I do not play video games" },Flagged = true},
                                new() {Option = new Option { Text = "1 hrs" } },
                                new() {Option = new Option { Text = "2 hrs" } },
                                new() {Option = new Option { Text = "4 hrs" } },
                                new() {Option = new Option { Text = "8 hrs" } },
                                new() {Option = new Option { Text = "16 hrs" } },
                                new() {Option = new Option { Text = "24 hrs" } }
                            }
                        }
                    }
                },
                new()
                {
                    Code = "B-2",
                    Description = "From B section 2",
                    OrderNumber = 2,
                    Questions = new List<Question>
                    {
                        new()
                        {
                            Code =  "B-2-1",
                            Text = "What is your goto news website?",
                            OrderNumber = 1,
                            QuestionType = QuestionType.SingleOptionWithText,
                            OptionsToQuestions = new List<OptionToQuestion>
                            {
                                new() {Option = new Option { Text = "bbc.com" }},
                                new() {Option = new Option { Text = "nytimes.com" }},
                                new() {Option = new Option { Text = "dailymail.co.uk" }, Flagged = true},
                                new() {Option = otherOption}
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
                NgoAdmins = ngo.Value.Admins.Select(x => new NgoAdmin
                {
                    Id = int.Parse(x.Key),
                    Account = x.Value.Account,
                    Password = _hashService.GetHash(x.Value.Password),

                }).ToArray(),
                Observers = ngo.Value.Observers.Select(x => new Observer
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
