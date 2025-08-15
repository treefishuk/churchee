using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Specifications;

namespace Churchee.Module.Site.Helpers
{
    public class StylesCompilerHelper
    {
        private readonly IDataStore _storage;
        private readonly ISassComplier _sassCompiler;

        public StylesCompilerHelper(IDataStore storage, ISassComplier sassCompiler)
        {
            _storage = storage;
            _sassCompiler = sassCompiler;
        }

        public async Task CompileStylesAsync(Guid applicationTenantId, string css, CancellationToken cancellationToken = default)
        {

            string compiledCss = await _sassCompiler.CompileStringAsync(css, true, cancellationToken);

            var entity = await _storage.GetRepository<Css>().FirstOrDefaultAsync(new CssForSpecifiedTenantSpecification(applicationTenantId), cancellationToken);

            entity.SetMinifiedStyles(compiledCss);

            await _storage.SaveChangesAsync(cancellationToken);

        }
    }
}