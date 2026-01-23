using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Churchee.Module.Tenancy.Features.Churches.Queries
{
    public class GetApplicationNameByIdQuery : IRequest<string>
    {
        public GetApplicationNameByIdQuery(Guid applicationTenantId)
        {
            ApplicationTenantId = applicationTenantId;
        }

        public Guid ApplicationTenantId { get; set; }
    }
}
