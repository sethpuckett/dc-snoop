using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dc_snoop.Models;
using dc_snoop.ViewModels;

namespace dc_snoop.Domain
{
    public interface ISnoopService
    {
        Person GetPerson(int id);

        Address GetAddress(int id);

        IEnumerable<SearchResult> Search(string term);
    }
}
