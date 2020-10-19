﻿using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Form.Mappers
{
    /// <summary>
    /// Base class using Template Design Pattern in order to facilitate the mapping of hierarchical objects.
    /// </summary>
    public abstract class HierarchicalMapperBase<TEntity, TDto, TChildEntity, TChildDto>
        where TEntity : IHierarchicalEntity<TChildEntity>
        where TDto : IHierarchicalEntity<TChildDto>
        where TChildEntity : IIdentifiableEntity
        where TChildDto : IIdentifiableEntity
    {
        private readonly IMapper _mapper;
        private readonly VoteMonitorContext _voteMonitorContext;

        protected HierarchicalMapperBase(IMapper mapper, VoteMonitorContext voteMonitorContext)
        {
            this._mapper = mapper;
            this._voteMonitorContext = voteMonitorContext;
        }

        public void Map(ref TEntity entity, TDto dto)
        {
            // If the entity is new in the dto (entity is null), we just map it
            // Otherwise, we update all properties except the hierarchy part(Excluded from profile).
            entity = entity == null
                    ? _mapper.Map<TEntity>(dto)
                    : _mapper.Map(dto, entity);

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
                Map(ref child, childDataItem.ChildDto);

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

        protected abstract void Map(ref TChildEntity childEntity, TChildDto childDto);
    }
}