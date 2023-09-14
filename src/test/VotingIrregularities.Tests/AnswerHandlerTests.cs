using Xunit;
using VoteMonitor.Api.Answer.Commands;
using VoteMonitor.Api.Answer.Handlers;
using VoteMonitor.Api.Answer.Models;

namespace VotingIrregularities.Tests;

public class AnswerHandlerTests
{
    [Fact]
    public void CanHandlerReduceAnswers()
    {
        var countyCode = "B";
        var pollingStationId = 1;
        var pollingStationNumber = 1;
        var lastModified = DateTime.Now;
        var answersBuilder = new List<AnswerDto>
        {
            new()
            {
                CountyCode = countyCode,
                PollingStationNumber = pollingStationNumber,
                PollingStationId = pollingStationId,
                QuestionId = 1,
                Options = new List<SelectedOptionDto>
                {
                    new() {OptionId  = 11, Value = "val0"},
                    new() {OptionId = 11, Value = "val234"},
                    new() {OptionId = 11, Value = "val2s34"},
                    new() {OptionId = 11, Value = "varl234"},
                    new() {OptionId = 11, Value = "varl234"},
                    new() {OptionId = 11, Value = "ok"},
                    new() {OptionId = 21, Value = "1"}
                }
            },
            new()
            {
                CountyCode = countyCode,
                PollingStationNumber = pollingStationNumber,
                PollingStationId = pollingStationId,
                QuestionId = 2,
                Options = new List<SelectedOptionDto>
                {
                    new() {OptionId = 21, Value = "val0"},
                    new() {OptionId = 21, Value = "val234"},
                    new() {OptionId = 21, Value = "ok"},
                    new() {OptionId = 22, Value = "ok"},
                    new() {OptionId = 23, Value = "ok"}
                }
            }
        };

        var message = new FillInAnswerCommand(1, answersBuilder);

        var reducedCollection = FillInAnswerQueryHandler.GetFlatListOfAnswers(message, lastModified);

        Assert.Equal(4, reducedCollection.Count);
        Assert.Equal(3, reducedCollection.Count(c=>c.IdOptionToQuestion > 20));
        Assert.Equal(1, reducedCollection.Count(c=>c.IdOptionToQuestion < 20));
        Assert.Equal("ok", reducedCollection.First(c=>c.IdOptionToQuestion == 11).Value);
        Assert.Equal("ok", reducedCollection.First(c=>c.IdOptionToQuestion == 21).Value);
    }
}
