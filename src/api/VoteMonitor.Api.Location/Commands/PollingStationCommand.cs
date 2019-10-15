using System.Collections.Generic;
using MediatR;
using MonitorizareVot.Api.Location.Models;

namespace VoteMonitor.Api.Location.Commands
{
    public class PollingStationCommand : IRequest<int>
    {
        public readonly List<PollingStationDTO> PollingStationsDTOs;
        public PollingStationCommand(List<PollingStationDTO> list)
        {
            PollingStationsDTOs = new List<PollingStationDTO>();
            foreach(PollingStationDTO element in list)
            {
                this.PollingStationsDTOs.Add(element);
            }
        }
    }
} 