using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Churchee.Module.Site.Features.Media.Queries
{
    public class GetMediaFoldersQueryResponseItem
    {
        public GetMediaFoldersQueryResponseItem(Guid id, string title, string path, bool hasChildren)
        {
            Id = id;
            Title = title;
            Path = path;
            Children = new List<GetMediaFoldersQueryResponseItem>();
            HasChildren = hasChildren;
        }

        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Path { get; set; }

        public bool HasChildren { get; set; }

        public IEnumerable<GetMediaFoldersQueryResponseItem> Children { get; set; }

    }
}
