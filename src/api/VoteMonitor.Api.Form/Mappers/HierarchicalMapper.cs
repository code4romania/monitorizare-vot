using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Form.Mappers
{
    /// <summary>
    /// Represents a helper class used to map an hierarchic entity from its DTO by either create or update.
    /// </summary>
    public class HierarchicalMapper<TEntity, TDto, TChildEntity, TChildDto> : IEntityMapper<TEntity, TDto>
        where TEntity : IHierarchicalEntity<TChildEntity>
        where TDto : IHierarchicalEntity<TChildDto>
        where TChildEntity : IIdentifiableEntity
        where TChildDto : IIdentifiableEntity
    {
        private readonly IUpdateOrCreateEntityMapper<TEntity, TDto> _updateOrCreateEntityMapper;
        private readonly IEntityMapper<TChildEntity, TChildDto> _updateOrCreateChildEntityMapper;

        public HierarchicalMapper(IUpdateOrCreateEntityMapper<TEntity, TDto> updateOrCreateEntityMapper, IEntityMapper<TChildEntity, TChildDto> updateOrCreateChildEntityMapper)
        {
            _updateOrCreateEntityMapper = updateOrCreateEntityMapper;
            _updateOrCreateChildEntityMapper = updateOrCreateChildEntityMapper;
        }

        public void Map(ref TEntity entity, TDto dto)
        {
            // We map the entity from dto using the update or create entity mapper.
            _updateOrCreateEntityMapper.Map(ref entity, dto);

            // In case the children property is null, we'll initialize it with an empty list.
            entity.Children ??= new List<TChildEntity>();

            // We get the left join from the dto's childtren and the entity's children
            var childData = from childDto in dto.Children ?? Enumerable.Empty<TChildDto>()
                            join child in entity.Children
                            on childDto.Id equals child.Id
                            into childDataJoined
                            from child in childDataJoined.DefaultIfEmpty()
                            select new { Child = child, ChildDto = childDto };

            var childrenToAdd = new List<TChildEntity>();

            foreach (var (childDataItem, childIndex) in childData.Select((childDataItem, childIndex) => (childDataItem, childIndex + 1)))
            {
                var child = childDataItem.Child;

                // We update the child property using the template method.
                _updateOrCreateChildEntityMapper.Map(ref child, childDataItem.ChildDto);

                // If the entity is ordered, we set the childIndex on it.
                if (child is IOrderedEntity orderedChildEntity)
                {
                    orderedChildEntity.OrderNumber = childIndex;
                }

                // In case the entity was added, we add it to the list of Children.
                if (childDataItem.Child == null)
                {
                    // We cannot add them directly to entity.Children as that collection is still enumerated.
                    childrenToAdd.Add(child);
                }
            }

            foreach (var child in childrenToAdd)
            {
                entity.Children.Add(child);
            }
        }
    }
}
