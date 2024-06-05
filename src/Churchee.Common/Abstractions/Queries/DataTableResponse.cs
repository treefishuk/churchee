using System.Collections.Generic;

namespace Churchee.Common.Abstractions
{
    public class DataTableResponse<T>
    {
        public int Draw { get; set; }
        public int RecordsTotal { get; set; }
        public int RecordsFiltered { get; set; }
        public IEnumerable<T> Data { get; set; }
        public string Error { get; set; }
    }
}
