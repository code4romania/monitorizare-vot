using MediatR;
using System.Collections.Generic;
using VoteMonitor.Api.Location.Models;
using VoteMonitor.Api.Location.Models.ResultValues;

namespace VoteMonitor.Api.Location.Commands
{
    public class PollingStationCommand : IRequest<PollingStationImportResultValue>
    {
        public readonly List<PollingStationDTO> PollingStationsDTOs;
        public PollingStationCommand(List<PollingStationDTO> list)
        {
            PollingStationsDTOs = new List<PollingStationDTO>();
            foreach (PollingStationDTO element in list)
            {
                this.PollingStationsDTOs.Add(element);
            }
        }
    }
}