namespace Churchee.Module.Tenancy.Features.Churches.Queries
{
    public class GetInfoForEditResponse
    {
        public GetInfoForEditResponse()
        {
            Domains = new List<string>();
        }

        public int? CharityNumber { get; set; }

        public List<string> Domains { get; set; }
    }
}
