using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.Storage;

namespace Churchee.Module.Site.Jobs
{
    public class ArticleImageProcessingJob : ImageProcessingJobBase
    {
        public ArticleImageProcessingJob(IDataStore dataStore, IBlobStore blobStore, IImageProcessor imageProcessor)
            : base(dataStore, blobStore, imageProcessor, "articles")
        {
        }
    }
}
