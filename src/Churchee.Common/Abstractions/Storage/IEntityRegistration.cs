using Microsoft.EntityFrameworkCore;

namespace Churchee.Common.Abstractions.Storage
{
    public interface IEntityRegistration
    {
        void RegisterEntities(ModelBuilder modelBuilder);
    }
}
