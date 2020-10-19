using VoteMonitor.Api.Form.Models;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Form.Mappers
{
    public interface IQuestionMapper
    {
        public void Map(ref Question form, QuestionDTO formDTO);
    }
}
