using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VotingIrregularities.Api.Models;
using VotingIrregularities.Domain.Models;

namespace VotingIrregularities.Api.Services
{
    public class PollingStationService : IPollingStationService
    {
        private readonly VotingContext _context;
        private readonly ILogger _logger;

        public PollingStationService(VotingContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int> GetPollingStationByCountyCode(int pollingStationNumber, string countyCode)
        {
            try
            {
                var countyId = _context.Counties.FirstOrDefault(c => c.Code == countyCode)?.Id;
                if (countyId == null)
                    throw new ArgumentException($"Could not find County with code: {countyCode}");

                return await GetPollingStationByCountyId(pollingStationNumber, countyId.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(new EventId(), ex.Message);
            }

            return -1;
        }

        public async Task<int> GetPollingStationByCountyId(int pollingStationNumber, int countyId)
        {
            try
            {
                var idSectie = await
                _context.PollingStations
                    .Where(a => a.IdCounty == countyId &&
                                a.Number == pollingStationNumber)
                    .Select(a => a.Id)
                    .ToListAsync();

                if (idSectie.Count == 0)
                    throw new ArgumentException($"No Polling station found for: {new { countyId, pollingStationNumber }}");


                if (idSectie.Count > 1) // TODO[bv] add unique constraint on PollingStations [CountyId, Number]
                    throw new ArgumentException($"More than one polling station found for: {new { countyId, idSectie }}");

                return idSectie.Single();
            }
            catch (Exception ex)
            {
                _logger.LogError(new EventId(), ex.Message);
            }

            return -1;
        }

        public async Task<IEnumerable<CountyPollingStationLimit>> GetPollingStationsAssignmentsForAllCounties()
        {
            return await _context.Counties
                .Select(c => new CountyPollingStationLimit { Name = c.Name, Code = c.Code, Limit = c.NumberOfPollingStations, Id = c.Id })
                .ToListAsync();
        }
    }
}
