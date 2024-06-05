using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Churchee.Common.Storage;
using Churchee.Module.Tenancy.Infrastructure;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace Churchee.Module.Site.Features.CDN.Queries
{
    public class GetCDNPathQueryHandler : IRequestHandler<GetCDNPathQuery, string>
    {
        private readonly IConfiguration _configuration;
        private readonly ITenantResolver _tenantResolver;

        public GetCDNPathQueryHandler(IConfiguration configuration, ITenantResolver tenantResolver)
        {
            _configuration = configuration;
            _tenantResolver = tenantResolver;
        }

        public async Task<string> Handle(GetCDNPathQuery request, CancellationToken cancellationToken)
        {
            string urlPrefix = _configuration.GetSection("Images")["Prefix"];

            string result = urlPrefix.Replace("*", _tenantResolver.GetTenantDevName());

            return await Task.FromResult(result);
        }
    }
}
