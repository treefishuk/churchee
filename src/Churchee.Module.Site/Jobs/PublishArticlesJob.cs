using Churchee.Common.Abstractions.Queue;
using Churchee.Common.Storage;
using Churchee.Module.Site.Specifications;

namespace Churchee.Module.Site.Jobs
{
    public class PublishArticlesJob : ISystemJob
    {

        private readonly IDataStore _dataStore;

        public PublishArticlesJob(IDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var articleRepo = _dataStore.GetRepository<Entities.Article>();

            var articlesToPublish = await articleRepo.GetListAsync(new ArticlesThatNeedPublishingSpecification(), cancellationToken);

            foreach (var article in articlesToPublish)
            {
                article.Publish();
            }

            await _dataStore.SaveChangesAsync(cancellationToken);
        }
    }
}
