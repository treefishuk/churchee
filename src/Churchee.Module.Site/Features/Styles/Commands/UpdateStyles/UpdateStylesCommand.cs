using Churchee.Common.ResponseTypes;
using MediatR;

namespace Churchee.Module.Site.Features.Styles.Commands
{
    public class UpdateStylesCommand : IRequest<CommandResponse>
    {
        public UpdateStylesCommand(string css)
        {
            Css = css;
        }

        public string Css { get; }

    }
}
