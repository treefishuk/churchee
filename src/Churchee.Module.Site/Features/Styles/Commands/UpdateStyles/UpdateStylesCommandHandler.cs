using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Queue;
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
        private readonly ILogger _logger;
        private readonly IJobService _jobService;
        private readonly ISassComplier _sassCompiler;

        public UpdateStylesCommandHandler(IDataStore storage, ICurrentUser currentUser, ILogger<UpdateStylesCommandHandler> logger, IJobService jobService, ISassComplier sassCompiler)
        {
            _storage = storage;
            _currentUser = currentUser;
            _logger = logger;
            _jobService = jobService;
            _sassCompiler = sassCompiler;
        }

        public async Task<CommandResponse> Handle(UpdateStylesCommand request, CancellationToken cancellationToken)
        {
            var response = new CommandResponse();

            var applicationTenantId = await _currentUser.GetApplicationTenantId();

            try
            {
                var css = _storage.GetRepository<Css>().GetQueryable().FirstOrDefault();

                if (css == null)
                {

                    css = new Css(applicationTenantId);

                    _storage.GetRepository<Css>().Create(css);
                }

                css.SetStyles(request.Css);

                string compiledCss = await _sassCompiler.CompileStringAsync(request.Css, true, cancellationToken);

                css.SetMinifiedStyles(compiledCss);

                await _storage.SaveChangesAsync(cancellationToken);

                return new CommandResponse();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to compile SCSS");

                response.AddError("Failed to compile SCSS", "Css");

                return response;
            }
        }
    }
}
