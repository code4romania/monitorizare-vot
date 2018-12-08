using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using VotingIrregularities.Domain.Models;
using VotingIrregularities.Domain.ValueObjects;

namespace VotingIrregularities.Api.Services
{
    public class VotingSectionDBService : IVotingSectionService
    {
        private readonly VotingContext _context;
        private readonly ILogger _logger;

        public VotingSectionDBService(VotingContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int> GetSingleVotingSection(string countyCode, int sectionNumber)
        {
            try
            {
                CountyEnum county;
                var j = Enum.TryParse(countyCode, true, out county);

                if (!j)
                    throw new ArgumentException($"County inexistent: {countyCode}");

                return await GetSingleVotingSection((int)county, sectionNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(new EventId(), ex.Message);
            }

            return -1;
        }

        public async Task<int> GetSingleVotingSection(int countyId, int sectionNumber)
        {
            try
            {
                var sectionId = await
                _context.VotingSections
                    .Where(
                        a =>
                            a.CountyId == countyId &&
                            a.SectionNumber == sectionNumber)
                    .Select(a => a.VotingSectionId)
                    .ToListAsync();

                if (sectionId.Count == 0)
                    throw new ArgumentException($"Section inexistenta pentru: {new { countyId = countyId, sectionNumber = sectionNumber }}");


                if (sectionId.Count > 1)
                    throw new ArgumentException($"S-au gasit mai multe sectii pentru: {new { countyId = countyId, sectionId = sectionId }}");

                return sectionId.Single();
            }
            catch (Exception ex)
            {
                _logger.LogError(new EventId(), ex.Message);
            }

            return -1;
        }
    }
}
