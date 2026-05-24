using Churchee.Common.ResponseTypes;
using Churchee.CQRS.Abstractions;

namespace Churchee.Module.Site.Features.Blog.Commands
{
    public class SeedDefaultArticlesListingIfNeededCommand : IRequest<CommandResponse>
    {
    }
}
