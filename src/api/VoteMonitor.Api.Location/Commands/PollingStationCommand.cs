using System.Collections.Generic;
using MediatR;
using MonitorizareVot.Api.Location.Models;

namespace MonitorizareVot.Api.Location.Commands
{
    public class PollingStationCommand : IRequest<int>
    {
        private readonly List<PollingStationDTO> pollingStationsDTOs;
        public PollingStationCommand(List<PollingStationDTO> list)
        {
            pollingStationsDTOs = new List<PollingStationDTO>();
            foreach(PollingStationDTO element in list)
            {
                this.pollingStationsDTOs.Add(element);
            }
        }

        public List<PollingStationDTO> PollingStationsDTOs => pollingStationsDTOs;
    }
} 