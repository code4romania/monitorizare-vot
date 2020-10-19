using VoteMonitor.Api.Form.Models;

namespace VoteMonitor.Api.Form.Mappers
{
    public interface IFormMapper
    {
        public void Map(ref Entities.Form form, FormDTO formDTO);
    }
}
