using AutoMapper;
using VoteMonitor.Api.Form.Models;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Form.Mappers
{
    public class QuestionMapper : HierarchicalMapperBase<Question, QuestionDTO, OptionToQuestion, OptionToQuestionDTO>, IQuestionMapper
    {
        private readonly IMapper _mapper;

        public QuestionMapper(IMapper mapper, VoteMonitorContext voteMonitorContext) : base(mapper, voteMonitorContext)
        {
            this._mapper = mapper;
        }

        protected override void Map(ref OptionToQuestion childEntity, OptionToQuestionDTO childDto)
        {
            childEntity = childEntity == null
                    ? _mapper.Map<OptionToQuestion>(childDto)
                    : _mapper.Map(childDto, childEntity);

        }
    }

}
