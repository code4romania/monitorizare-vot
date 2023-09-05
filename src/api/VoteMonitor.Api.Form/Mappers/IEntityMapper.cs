namespace VoteMonitor.Api.Form.Mappers;

public interface IEntityMapper<TEntity, TDto>
{
    void Map(ref TEntity entity, TDto dto);
}