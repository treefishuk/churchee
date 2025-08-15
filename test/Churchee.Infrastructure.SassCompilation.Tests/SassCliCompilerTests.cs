namespace Churchee.Infrastructure.SassCompilation.Tests
{
    public class SassCliCompilerTests
    {
        private readonly SassCliCompiler _cliService;

        public SassCliCompilerTests()
        {
            _cliService = new SassCliCompiler();
        }

        [Fact]
        public async Task CanCompile_BootstrapWithOverrides()
        {
            string dbVars = "$primary: #4a148c; $body-bg: #f7f7fb;";

            string dbCustom = "body { font-family: Arial; }";

            string css = await _cliService.CompileStringAsync(
                dbVars + "\n@import \"bootstrap\";\n" + dbCustom,
                //dbVars + dbCustom,
                compressed: true
            );

            Assert.Contains("body", css);
            Assert.Contains("#4a148c", css); // Our primary override
        }
    }
}
