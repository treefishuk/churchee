﻿using Churchee.Common.Abstractions.Auth;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Churchee.Module.Site.Features.Pages.Commands.CreatePage
{
    public class CreatePageCommandHandler : IRequestHandler<CreatePageCommand, CommandResponse>
    {

        private readonly IDataStore _storage;
        private readonly ICurrentUser _currentUser;

        public CreatePageCommandHandler(IDataStore storage, ICurrentUser currentUser)
        {
            _storage = storage;
            _currentUser = currentUser;
        }

        public async Task<CommandResponse> Handle(CreatePageCommand request, CancellationToken cancellationToken)
        {
            var applicationTenantId = await _currentUser.GetApplicationTenantId();

            var repo = _storage.GetRepository<Page>();

            string path = await GetUrlPath(request.ParentId);

            string slug = request.Title.ToURL();

            string url = $"{path}/{slug}";

            var newPage = new Page(applicationTenantId, request.Title, url, request.Description, request.PageTypeId, request.ParentId, true);

            repo.Create(newPage);

            await _storage.SaveChangesAsync(cancellationToken);

            return new CommandResponse();
        }

        private async Task<string> GetUrlPath(Guid? parentId)
        {
            if (parentId == null)
            {
                return string.Empty;
            }

            var repo = _storage.GetRepository<Page>();

            string parentSlug =  await repo.GetQueryable().Where(w => w.Id == parentId.Value).Select(s => s.Url).FirstOrDefaultAsync();

            return parentSlug;
        }
    }
}
