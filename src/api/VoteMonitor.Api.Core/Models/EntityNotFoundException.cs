using System;

namespace VoteMonitor.Api.Core.Models
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(Type type, object objectId) : base($"The {type.GetType()} entity has no entity with id: {objectId}")
        {
        }
    }
}
