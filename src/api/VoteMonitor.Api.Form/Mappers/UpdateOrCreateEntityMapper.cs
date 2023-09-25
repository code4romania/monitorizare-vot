using AutoMapper;

namespace VoteMonitor.Api.Form.Mappers;

public class UpdateOrCreateEntityMapper<TEntity, TDto> : IUpdateOrCreateEntityMapper<TEntity, TDto>
{
    private readonly IMapper _mapper;

    public UpdateOrCreateEntityMapper(IMapper mapper)
    {
        _mapper = mapper;
    }

    public virtual void Map(ref TEntity entity, TDto dto)
    {
        // If the entity is new in the dto (entity is null), we just map it
        // Otherwise, we update all properties except the hierarchy part(Excluded from profile).
        entity = entity == null
            ? _mapper.Map<TEntity>(dto)
            : _mapper.Map(dto, entity);
    }
}