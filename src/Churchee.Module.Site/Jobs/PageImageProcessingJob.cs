using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.Storage;
using Hangfire;

namespace Churchee.Module.Site.Jobs
{
    public class PageImageProcessingJob : ImageProcessingJobBase
    {
        public PageImageProcessingJob(IDataStore dataStore, IBlobStore blobStore, IImageProcessor imageProcessor, IBackgroundJobClient backgroundJobClient)
            : base(dataStore, blobStore, imageProcessor, "pages")
        {
        }
    }
}
