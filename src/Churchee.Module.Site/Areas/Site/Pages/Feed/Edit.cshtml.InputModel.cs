using System.ComponentModel.DataAnnotations;

namespace Churchee.Module.Site.Areas.Management.Pages.Blog.Edit
{
    public class InputModel
    {
        public string Title { get; set; }

        public string Slug { get; set; }

        public string Description { get; set; }

        [DataType("WYSIWYG")]
        public string Content { get; set; }
    }
}
