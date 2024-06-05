using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using MediatR;

namespace Churchee.Module.Site.Features.Media.Commands
{
    public class DeleteMediaItemCommandHandler : IRequestHandler<DeleteMediaItemCommand, CommandResponse>
    {
        private readonly IDataStore _storage;

        public DeleteMediaItemCommandHandler(IDataStore storage)
        {
            _storage = storage;
        }

        public async Task<CommandResponse> Handle(DeleteMediaItemCommand request, CancellationToken cancellationToken)
        {
            await _storage.GetRepository<MediaItem>().SoftDelete(request.MediaItemId);

            await _storage.SaveChangesAsync(cancellationToken);

            return new CommandResponse();
        }
    }
}
