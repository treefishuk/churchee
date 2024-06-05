using Churchee.Module.Site.Areas.Management.Pages.Blog.Edit;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Churchee.Module.Site.Areas.Management.Pages.Blog
{
    public class EditModel : PageModel
    {

        private readonly IMediator _mediator;

        [BindProperty]
        public InputModel InputModel { get; set; }

        public EditModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task OnGetAsync()
        {



        }
    }
}
