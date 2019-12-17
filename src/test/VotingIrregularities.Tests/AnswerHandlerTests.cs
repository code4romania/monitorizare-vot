using System;
using System.Collections.Generic;
using VotingIrregularities.Domain.RaspunsAggregate;
using VotingIrregularities.Domain.RaspunsAggregate.Commands;
using Xunit;
using System.Linq;

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
            var codFormular = "T";
            var lastModified = DateTime.Now;

            var message = new CompleteazaRaspunsCommand
            {
                IdObservator = 1,
                Raspunsuri = new List<ModelRaspuns>()
            };

            message.Raspunsuri.Add(new ModelRaspuns
            {
                CodJudet = codJudet,
                NumarSectie = nrSectie,
                IdSectie = idSectie,
                CodFormular = codFormular,
                IdIntrebare = 1,
                Optiuni = new List<ModelOptiuniSelectate>
                {
                    new ModelOptiuniSelectate{IdOptiune = 11, Value = "val0"},
                    new ModelOptiuniSelectate{IdOptiune = 11, Value = "val234"},
                    new ModelOptiuniSelectate{IdOptiune = 11, Value = "val2s34"},
                    new ModelOptiuniSelectate{IdOptiune = 11, Value = "varl234"},
                    new ModelOptiuniSelectate{IdOptiune = 11, Value = "varl234"},
                    new ModelOptiuniSelectate{IdOptiune = 11, Value = "ok"},
                    new ModelOptiuniSelectate{IdOptiune = 21, Value = "1"}

                }
            });
            message.Raspunsuri.Add(new ModelRaspuns
            {
                CodJudet = codJudet,
                NumarSectie = nrSectie,
                IdSectie = idSectie,
                CodFormular = codFormular,
                IdIntrebare = 2,
                Optiuni = new List<ModelOptiuniSelectate>
                {
                    new ModelOptiuniSelectate{IdOptiune = 21, Value = "val0"},
                    new ModelOptiuniSelectate{IdOptiune = 21, Value = "val234"},
                    new ModelOptiuniSelectate{IdOptiune = 21, Value = "ok"},
                    new ModelOptiuniSelectate{IdOptiune = 22, Value = "ok"},
                    new ModelOptiuniSelectate{IdOptiune = 23, Value = "ok"}
                }
            });

            var reducedCollection = CompleteazaRaspunsHandler.GetFlatListOfAnswers(message, lastModified);

            Assert.Equal(4, reducedCollection.Count);
            Assert.Equal(3, reducedCollection.Count(c => c.IdOptionToQuestion > 20));
            Assert.Equal(1, reducedCollection.Count(c => c.IdOptionToQuestion < 20));
            Assert.Equal("ok", reducedCollection.First(c => c.IdOptionToQuestion == 11).Value);
            Assert.Equal("ok", reducedCollection.First(c => c.IdOptionToQuestion == 21).Value);
        }
    }
}
