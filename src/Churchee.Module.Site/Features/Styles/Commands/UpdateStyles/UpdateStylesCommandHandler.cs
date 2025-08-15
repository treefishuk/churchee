using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Churchee.Module.Site.Features.Styles.Commands
{
    public class UpdateStylesCommandHandler : IRequestHandler<UpdateStylesCommand, CommandResponse>
    {

        private readonly IDataStore _storage;
        private readonly ICurrentUser _currentUser;
        private readonly ISassComplier _sassCompiler;
        private readonly ILogger _logger;

        public UpdateStylesCommandHandler(IDataStore storage, ICurrentUser currentUser, ISassComplier sassCompiler, ILogger<UpdateStylesCommandHandler> logger)
        {
            _storage = storage;
            _currentUser = currentUser;
            _sassCompiler = sassCompiler;
            _logger = logger;
        }

        public async Task<CommandResponse> Handle(UpdateStylesCommand request, CancellationToken cancellationToken)
        {
            var response = new CommandResponse();

            string compiledCss;

            try
            {
                string bootstrapPath = Path.Combine("wwwroot", "lib", "bootstrap", "scss");

                compiledCss = await _sassCompiler.CompileStringAsync(request.Css, ["bootstrapPath"], true, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to compile SCSS");

                response.AddError("Failed to compile SCSS", "Css");

                return response;
            }

            var css = _storage.GetRepository<Css>().GetQueryable().FirstOrDefault();

            if (css == null)
            {
                var applicationTenantId = await _currentUser.GetApplicationTenantId();

                css = new Css(applicationTenantId);

                _storage.GetRepository<Css>().Create(css);
            }

            css.SetStyles(request.Css);

            css.SetMinifiedStyles(compiledCss);

            await _storage.SaveChangesAsync(cancellationToken);

            return new CommandResponse();
        }
    }
}
