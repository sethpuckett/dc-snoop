using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace dc_snoop.ViewModels
{
    public enum SearchResultType
    {
        [Display(Name = "PERSON")]
        Person,
        [Display(Name = "ADDRESS")]
        Address
    }
}
