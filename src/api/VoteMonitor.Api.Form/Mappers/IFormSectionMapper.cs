using VoteMonitor.Api.Form.Models;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Form.Mappers
{
    public interface IFormSectionMapper
    {
        public void Map(ref FormSection form, FormSectionDTO formDTO);
    }
}
