namespace VoteMonitor.Api.Form.Mappers
{
    public interface IUpdateOrCreateEntityMapper<TEntity, TDto>
    {
        void Map(ref TEntity entity, TDto dto);
    }
}