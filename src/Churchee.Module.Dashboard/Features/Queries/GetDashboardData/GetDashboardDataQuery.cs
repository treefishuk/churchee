using MediatR;

namespace Churchee.Module.Dashboard.Features.Queries
{
    public class GetDashboardDataQuery : IRequest<GetDashboardDataResponse>
    {
        public GetDashboardDataQuery(int days)
        {
            Days = days;
        }

        public int Days { get; set; }
    }
}
