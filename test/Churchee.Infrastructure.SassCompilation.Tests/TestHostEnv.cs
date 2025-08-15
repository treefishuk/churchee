using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;

namespace Churchee.Infrastructure.SassCompilation.Tests
{
    internal class TestHostEnv : IWebHostEnvironment
    {
        public TestHostEnv()
        {
            WebRootPath = string.Empty;
            ContentRootPath = string.Empty;
            ApplicationName = "Tests";
            EnvironmentName = "Development";
        }

        public string ApplicationName { get; set; }
        public IFileProvider WebRootFileProvider { get; set; }
        public string WebRootPath { get; set; }
        public string EnvironmentName { get; set; }
        public string ContentRootPath { get; set; }
        public IFileProvider ContentRootFileProvider { get; set; }
    }
}
