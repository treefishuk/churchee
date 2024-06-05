namespace Churchee.Module.Tenancy.Infrastructure
{
    public interface ITenantResolver
    {
        Guid GetTenantId();

        string GetTenantDevName();

    }
}
