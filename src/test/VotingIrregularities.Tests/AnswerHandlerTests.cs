using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using VoteMonitor.Api.Answer.Commands;
using VoteMonitor.Api.Answer.Handlers;
using VoteMonitor.Api.Answer.Models;

namespace VotingIrregularities.Tests
{
    public class AnswerHandlerTests
    {
        [Fact]
        public void CanHandlerReduceAnswers()
        {
            var countyCode = "B";
            var pollingStationId = 1;
            var pollingStationNumber = 1;
            var lastModified = DateTime.Now;

            var message = new FillInAnswerCommand
            {
                ObserverId = 1,
                Answers = new List<AnswerDto>()
            };

            message.Answers.Add(new AnswerDto
            {
                CountyCode = countyCode,
                PollingStationNumber = pollingStationNumber,
                PollingStationId = pollingStationId,
                QuestionId = 1,
                Options = new List<SelectedOptionDto>
                {
                    new SelectedOptionDto{OptionId  = 11, Value = "val0"},
                    new SelectedOptionDto{OptionId = 11, Value = "val234"},
                    new SelectedOptionDto{OptionId = 11, Value = "val2s34"},
                    new SelectedOptionDto{OptionId = 11, Value = "varl234"},
                    new SelectedOptionDto{OptionId = 11, Value = "varl234"},
                    new SelectedOptionDto{OptionId = 11, Value = "ok"},
                    new SelectedOptionDto{OptionId = 21, Value = "1"}
                }
            });
            message.Answers.Add(new AnswerDto
            {
                CountyCode = countyCode,
                PollingStationNumber = pollingStationNumber,
                PollingStationId = pollingStationId,
                QuestionId = 2,
                Options = new List<SelectedOptionDto>
                {
                    new SelectedOptionDto{OptionId = 21, Value = "val0"},
                    new SelectedOptionDto{OptionId = 21, Value = "val234"},
                    new SelectedOptionDto{OptionId = 21, Value = "ok"},
                    new SelectedOptionDto{OptionId = 22, Value = "ok"},
                    new SelectedOptionDto{OptionId = 23, Value = "ok"}
                }
            });

            var reducedCollection = FillInAnswerQueryHandler.GetFlatListOfAnswers(message, lastModified);

            Assert.Equal(4, reducedCollection.Count);
            Assert.Equal(3, reducedCollection.Count(c=>c.IdOptionToQuestion > 20));
            Assert.Equal(1, reducedCollection.Count(c=>c.IdOptionToQuestion < 20));
            Assert.Equal("ok", reducedCollection.First(c=>c.IdOptionToQuestion == 11).Value);
            Assert.Equal("ok", reducedCollection.First(c=>c.IdOptionToQuestion == 21).Value);
        }
    }
}
