using MediatR;

namespace VoteMonitor.Api.Observer.Commands
{
    public class ObserverGenerateProperties : IRequest<ObserverGenerateCommand>
    {
        public ObserverGenerateProperties(int number)
        {
            NrObservers = number;
        }

        public int NrObservers { get; set; }
        public int IdNgo { get; set; }
    }
}
