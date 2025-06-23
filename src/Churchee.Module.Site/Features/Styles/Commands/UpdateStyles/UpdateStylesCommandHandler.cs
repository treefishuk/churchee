using Churchee.Common.Abstractions.Auth;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Helpers;
using MediatR;

namespace Churchee.Module.Site.Features.Styles.Commands
{
    public class UpdateStylesCommandHandler : IRequestHandler<UpdateStylesCommand, CommandResponse>
    {

        private readonly IDataStore _storage;
        private readonly ICurrentUser _currentUser;

        public UpdateStylesCommandHandler(IDataStore storage, ICurrentUser currentUser)
        {
            _storage = storage;
            _currentUser = currentUser;
        }

        public async Task<CommandResponse> Handle(UpdateStylesCommand request, CancellationToken cancellationToken)
        {
            var css = _storage.GetRepository<Css>().GetQueryable().FirstOrDefault();

            if (css == null)
            {
                var applicationTenantId = await _currentUser.GetApplicationTenantId();

                css = new Css(applicationTenantId);

                _storage.GetRepository<Css>().Create(css);
            }

            css.SetStyles(request.Css);

            using var reader = new StringReader(css.Styles);

            var minifier = new CssMinifier();

            css.SetMinifiedStyles(minifier.Minify(reader));

            await _storage.SaveChangesAsync(cancellationToken);

            return new CommandResponse();
        }
    }
}
