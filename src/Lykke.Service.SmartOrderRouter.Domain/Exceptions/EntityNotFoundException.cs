using System;

namespace Lykke.Service.SmartOrderRouter.Domain.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException()
            : base("Entity not found")
        {
        }
    }
}
