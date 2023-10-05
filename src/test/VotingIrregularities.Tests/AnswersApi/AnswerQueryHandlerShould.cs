using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;
using VoteMonitor.Api.Answer.Commands;
using VoteMonitor.Api.Answer.Handlers;
using VoteMonitor.Api.Answer.Models;
using VoteMonitor.Api.Core.Services;
using VoteMonitor.Api.Location.Services;
using VoteMonitor.Entities;
using Xunit;

namespace VotingIrregularities.Tests.AnswersApi;

public class AnswerQueryHandlerShould
{
    private readonly DbContextOptions<VoteMonitorContext> _dbContextOptions;
    private readonly NoCacheService _noCacheService = new();
    private readonly NullLogger<PollingStationService> _nullLogger = NullLogger<PollingStationService>.Instance;

    public AnswerQueryHandlerShould()
    {
        _dbContextOptions = new DbContextOptionsBuilder<VoteMonitorContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(warningsConfigurationBuilderAction: x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        SetupPollingStations();
    }

    private void SetupPollingStations()
    {
        using (var context = new VoteMonitorContext(options: _dbContextOptions))
        {
            context.PollingStations.Add(entity: new PollingStation()
            {
                Id = 1,
                Address = "An address",
                Number = 1,
                Municipality = new Municipality()
                {
                    Id = 1,
                    Code = "M1",
                    Name = "Municipality1",
                    County = new County()
                    {
                        Id = 1,
                        Name = "County1",
                        Code = "C1",
                        Province = new Province() { Id = 1, Code = "P1", Name = "Province 1" }
                    }
                }
            });

            context.PollingStations.Add(entity: new PollingStation()
            {
                Id = 2,
                Address = "An address",
                Number = 1,
                Municipality = new Municipality()
                {
                    Id = 2,
                    Code = "M2",
                    Name = "Municipality2",
                    County = new County()
                    {
                        Id = 2,
                        Name = "County2",
                        Code = "C2",
                        Province = new Province() { Id = 2, Code = "P2", Name = "Province 2" }
                    }
                }
            });

            context.SaveChanges();
        }
    }

    [Theory]
    [MemberData(memberName: nameof(AnswerQueryHandlerTestData))]
    public async Task ReturnCommandsWithAnswersWithPollingStationId(BulkAnswers command, int expectedCount, int[] expectedPollingStationsIds)
    {
        var voteMonitorContext = new VoteMonitorContext(options: _dbContextOptions);
        var sut = new AnswerQueryHandler(new PollingStationService(voteMonitorContext, _noCacheService, _nullLogger), voteMonitorContext);

        var result = await sut.Handle(message: command, cancellationToken: CancellationToken.None);

        result.Answers.Count.ShouldBe(expected: expectedCount);
        result.Answers.Select(selector: x => x.PollingStationId).ShouldBe(expected: expectedPollingStationsIds);
    }

    public static IEnumerable<object[]> AnswerQueryHandlerTestData =>
        new List<object[]>
        {
            new object[]
            {
                new BulkAnswers(observerId: 1,
                    answers: new BulkAnswerDto[]
                    {
                        new()
                        {
                            CountyCode = "C1",
                            MunicipalityCode = "M1",
                            PollingStationNumber = 1,
                            QuestionId = 1,
                            Options = new List<SelectedOptionDto> { new() { OptionId = 1} }
                        },
                        new()
                        {
                            CountyCode = "C1",
                            MunicipalityCode = "M1",
                            PollingStationNumber = 1,
                            QuestionId = 2,
                            Options = new List<SelectedOptionDto> { new() { OptionId = 2 } }
                        }
                    }),
                2, new[] { 1, 1 }
            },
            new object[]
            {
                new BulkAnswers(observerId: 1,
                    answers: new BulkAnswerDto[]
                    {
                        new()
                        {
                            CountyCode = "C1",
                            MunicipalityCode = "M1",
                            PollingStationNumber = 1,
                            QuestionId = 1,
                            Options = new List<SelectedOptionDto> { new() { OptionId = 1} }
                        },
                        new()
                        {
                            CountyCode = "C2",
                            MunicipalityCode = "M2",
                            PollingStationNumber = 1,
                            QuestionId = 2,
                            Options = new List<SelectedOptionDto>
                            {
                                new() { OptionId = 2 }, new() { OptionId = 3 }, new() { OptionId = 4 }
                            }
                        }
                    }),
                2, new[] { 1, 2 }
            },
            new object[]
            {
                new BulkAnswers(observerId: 1,
                    answers: new BulkAnswerDto[]
                    {
                        new()
                        {
                            CountyCode = "C2",
                            MunicipalityCode = "M2",
                            PollingStationNumber = 1,
                            QuestionId = 1,
                            Options = new List<SelectedOptionDto> { new() { OptionId = 1} }
                        },
                        new()
                        {
                            CountyCode = "C1",
                            MunicipalityCode = "M1",
                            PollingStationNumber = 1,
                            QuestionId = 2,
                            Options = new List<SelectedOptionDto>
                            {
                                new() { OptionId = 2 }, new() { OptionId = 3 }, new() { OptionId = 4 }
                            }
                        }
                    }),
                2, new[] { 2, 1 }
            },
            new object[]
            {
                new BulkAnswers(observerId: 1,
                    answers: new BulkAnswerDto[]
                    {
                        new()
                        {
                            CountyCode = "C2",
                            MunicipalityCode = "M2",
                            PollingStationNumber = 1,
                            QuestionId = 1,
                            Options = new List<SelectedOptionDto> { new() { OptionId = 1} }
                        },
                        new()
                        {
                            CountyCode = "C2",
                            MunicipalityCode = "M2",
                            PollingStationNumber = 1,
                            QuestionId = 2,
                            Options = new List<SelectedOptionDto> { new() { OptionId = 2 } }
                        }
                    }),
                2, new[] { 2, 2 }
            },
            new object[]
            {
                new BulkAnswers(observerId: 1,
                    answers: new BulkAnswerDto[]
                    {
                        new()
                        {
                            CountyCode = "C2",
                            MunicipalityCode = "M2",
                            PollingStationNumber = 1,
                            QuestionId = 1,
                            Options = new List<SelectedOptionDto> { new() { OptionId = 1} }
                        },
                        new()
                        {
                            CountyCode = "C1",
                            MunicipalityCode = "M1",
                            PollingStationNumber = 444, // polling station with such number does not exist
                            QuestionId = 2,
                            Options = new List<SelectedOptionDto> { new() { OptionId = 2 } }
                        }
                    }),
                1, new[] { 2 }
            },
            new object[]
            {
                new BulkAnswers(observerId: 1,
                    answers: new BulkAnswerDto[]
                    {
                        new()
                        {
                            CountyCode = "C1",
                            MunicipalityCode = "M1",
                            PollingStationNumber = 444, // polling station with such number does not exist
                            QuestionId = 1,
                            Options = new List<SelectedOptionDto> { new() { OptionId = 1} }
                        },
                        new()
                        {
                            CountyCode = "C1",
                            MunicipalityCode = "M1",
                            PollingStationNumber = 444, // polling station with such number does not exist
                            QuestionId = 2,
                            Options = new List<SelectedOptionDto> { new() { OptionId = 2 } }
                        }
                    }),
                0, Array.Empty<int>()
            },
        };
}
