using Microsoft.EntityFrameworkCore;

namespace Churchee.Common.Abstractions.Storage
{
    public interface IFrontEndEntityRegistration
    {
        void RegisterEntities(ModelBuilder modelBuilder);
    }
}
