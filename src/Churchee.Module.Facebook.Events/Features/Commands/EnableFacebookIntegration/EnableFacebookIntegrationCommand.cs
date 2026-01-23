using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Churchee.Common.ResponseTypes;
using MediatR;

namespace Churchee.Module.Facebook.Events.Features.Commands
{
    public class EnableFacebookIntegrationCommand : IRequest<CommandResponse>
    {
        public EnableFacebookIntegrationCommand(string token, string domain)
        {
            Token = token;
            Domain = domain;
        }

        public string Token { get; set; }
        public string Domain { get; set; }

    }
}
