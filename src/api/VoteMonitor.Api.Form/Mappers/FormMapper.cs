using AutoMapper;
using VoteMonitor.Api.Form.Models;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Form.Mappers
{
    public class FormMapper : HierarchicalMapperBase<Entities.Form, FormDTO, FormSection, FormSectionDTO>, IFormMapper
    {
        private IFormSectionMapper _formSectionMapper;

        public FormMapper(IFormSectionMapper formSectionMapper, IMapper mapper, VoteMonitorContext voteMonitorContext) : base(mapper, voteMonitorContext)
        {
            this._formSectionMapper = formSectionMapper;
        }

        protected override void Map(ref FormSection childEntity, FormSectionDTO childDto)
        {
            _formSectionMapper.Map(ref childEntity, childDto);
        }
    }
}
