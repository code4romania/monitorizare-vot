using AutoMapper;
using VoteMonitor.Api.Form.Models;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Form.Mappers
{
    public class FormSectionMapper : HierarchicalMapperBase<FormSection, FormSectionDTO, Question, QuestionDTO>, IFormSectionMapper
    {
        private IQuestionMapper _questionMapper;

        public FormSectionMapper(IQuestionMapper questionMapper, IMapper mapper, VoteMonitorContext voteMonitorContext) : base(mapper, voteMonitorContext)
        {
            this._questionMapper = questionMapper;
        }

        protected override void Map(ref Question childEntity, QuestionDTO childDto)
        {
            _questionMapper.Map(ref childEntity, childDto);
        }
    }
}