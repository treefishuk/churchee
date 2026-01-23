using Churchee.Common.ResponseTypes;
using MediatR;

namespace Churchee.Module.Site.Features.Blog.Commands
{
    public class SeedDefaultArticlesListingIfNeededCommand : IRequest<CommandResponse>
    {
    }
}
