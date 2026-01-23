using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Site.Areas.Site.Models;
using Churchee.Module.Site.Entities;
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
            var existingItems = await _storage.GetRepository<PageTypeContent>()
                .GetListAsync(new PageContentTypesForPageType(request.PageTypeId), cancellationToken);

            ProcessNewAndUpdatedItems(request, existingItems);

            ProcessRemovedItems(request, existingItems);

            await _storage.SaveChangesAsync(cancellationToken);

            return new CommandResponse();
        }

        private void ProcessNewAndUpdatedItems(UpdatePageTypeContentCommand request, List<PageTypeContent> existingItems)
        {
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
        }

        private void ProcessRemovedItems(UpdatePageTypeContentCommand request, List<PageTypeContent> existingItems)
        {
            foreach (var item in existingItems.Where(w => !request.Content.Select(s => s.Id).Contains(w.Id)))
            {
                DeleteItem(item.Id);
            }
        }

        private static void UpdateExistingItem(PageTypeContent existing, PageTypeContentItemModel item)
        {
            existing.UpdateDetails(item.Required, item.Name, item.Type.Value, item.Order);
        }

        private void AddNewItem(Guid pageTypeId, PageTypeContentItemModel item)
        {
            var pageType = _storage
                .GetRepository<PageType>()
                .ApplySpecification(new PageTypeWithPageTypeContentSpecification(pageTypeId))
                .FirstOrDefault();

            if (pageType == null)
            {
                return;
            }

            pageType.AddPageTypeContent(item.Id, item.Name, item.Type.Value, item.Required, item.Order);
        }

        private void DeleteItem(Guid itemId)
        {
            _storage.GetRepository<PageTypeContent>().SoftDelete(itemId);
        }
    }
}
