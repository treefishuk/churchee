using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Queue;
using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Helpers;
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
        private readonly IJobService _jobService;

        public UpdateStylesCommandHandler(IDataStore storage, ICurrentUser currentUser, ISassComplier sassCompiler, ILogger<UpdateStylesCommandHandler> logger, IJobService jobService)
        {
            _storage = storage;
            _currentUser = currentUser;
            _sassCompiler = sassCompiler;
            _logger = logger;
            _jobService = jobService;
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

                await _storage.SaveChangesAsync(cancellationToken);

                _jobService.QueueJob<StylesCompilerHelper>(s => s.CompileStylesAsync(applicationTenantId, request.Css, CancellationToken.None));

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
