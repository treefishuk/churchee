using Microsoft.Extensions.Configuration;

namespace Churchee.Infrastructure.SassCompilation.Tests
{
    public class SassCliCompilerTests
    {
        private readonly SassCliCompiler _cliService;

        public SassCliCompilerTests()
        {
            // Test environment points to bin output
            var env = new TestHostEnv
            {
                ContentRootPath = Directory.GetCurrentDirectory(),
                WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")
            };
            _cliService = new SassCliCompiler(env, new ConfigurationBuilder().Build());
        }

        [Fact]
        public async Task CanCompile_BootstrapWithOverrides()
        {
            string dbVars = "$primary: #4a148c; $body-bg: #f7f7fb;";
            string dbCustom = "body { font-family: Arial; }";

            string bootstrapPath = Path.Combine("wwwroot", "lib", "bootstrap", "scss");

            string css = await _cliService.CompileStringAsync(
                dbVars + "\n@import \"bootstrap\";\n" + dbCustom,
                [bootstrapPath],
                compressed: true
            );

            Assert.Contains("body", css);
            Assert.Contains("#4a148c", css); // Our primary override
        }
    }
}
