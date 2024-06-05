using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Site.Areas.Site.Models;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Features.PageTypes.Queries;
using Churchee.Module.Site.Specifications;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Churchee.Module.Site.Features.PageTypes.Commands.UpdatePageTypeContent
{
    public class UpdatePageTypeContentCommandHandler : IRequestHandler<UpdatePageTypeContentCommand, CommandResponse>
    {

        private readonly IDataStore _storage;

        public UpdatePageTypeContentCommandHandler(IDataStore storage)
        {
            _storage = storage;
        }

        public async Task<CommandResponse> Handle(UpdatePageTypeContentCommand request, CancellationToken cancellationToken)
        {
            var existingItems = await _storage.GetRepository<WebContentTypeContent>()
                .ApplySpecification(new PageContentTypesForPageType(request.PageTypeId))
                .ToListAsync();

            foreach (var item in request.Content)
            {
                var existingData = existingItems.FirstOrDefault(a => a.Id == item.Id);

                if (existingData != null)
                {
                    UpdateExistingItem(existingData, item);
                }

                if (existingData == null)
                {
                    AddNewItem(request.PageTypeId, item);
                }

            }

            await _storage.SaveChangesAsync(cancellationToken);

            return new CommandResponse();
        }

        private void UpdateExistingItem(WebContentTypeContent existing, PageTypeContentItemModel item)
        {
            existing.UpdateDetails(item.Required, item.Name, item.Type.Value, item.Order);
        }

        private void AddNewItem(Guid pageTypeId, PageTypeContentItemModel item)
        {
            var pageType = _storage.GetRepository<PageType>().GetById(pageTypeId);

            pageType.AddPageTypeContent(item.Id, item.Name, item.Type.Value, item.Required);

        }
    }
}
