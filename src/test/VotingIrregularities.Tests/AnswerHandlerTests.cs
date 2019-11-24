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
            //var handler = new CompleteazaRaspunsHandler(new VotingContext(), new Mapper(), null);
            var codJudet = "B";
            var idSectie = 1;
            var nrSectie = 1;
            //var codFormular = "T";
            var lastModified = DateTime.Now;

            var message = new CompleteazaRaspunsCommand
            {
                ObserverId = 1,
                Answers = new List<AnswerDTO>()
            };

            message.Answers.Add(new AnswerDTO
            {
                CountyCode = codJudet,
                PollingStationNumber = nrSectie,
                PollingSectionId = idSectie,
                //CodFormular = codFormular,
                QuestionId = 1,
                Options = new List<SelectedOptionModel>
                {
                    new SelectedOptionModel{OptionId  = 11, Value = "val0"},
                    new SelectedOptionModel{OptionId = 11, Value = "val234"},
                    new SelectedOptionModel{OptionId = 11, Value = "val2s34"},
                    new SelectedOptionModel{OptionId = 11, Value = "varl234"},
                    new SelectedOptionModel{OptionId = 11, Value = "varl234"},
                    new SelectedOptionModel{OptionId = 11, Value = "ok"},
                    new SelectedOptionModel{OptionId = 21, Value = "1"}
                    
                }
            });
            message.Answers.Add(new AnswerDTO
            {
                CountyCode = codJudet,
                PollingStationNumber = nrSectie,
                PollingSectionId = idSectie,
               // CodFormular = codFormular,
                QuestionId = 2,
                Options = new List<SelectedOptionModel>
                {
                    new SelectedOptionModel{OptionId = 21, Value = "val0"},
                    new SelectedOptionModel{OptionId = 21, Value = "val234"},
                    new SelectedOptionModel{OptionId = 21, Value = "ok"},
                    new SelectedOptionModel{OptionId = 22, Value = "ok"},
                    new SelectedOptionModel{OptionId = 23, Value = "ok"}
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
