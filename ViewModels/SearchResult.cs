using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dc_snoop.ViewModels
{
    public class SearchResult
    {
        public SearchResultType Type { get; set; }

        public int Id { get; set; }

        public string Text { get; set; }

        public int Strength { get; set; }
    }
}
