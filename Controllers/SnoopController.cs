using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dc_snoop.DAL;
using dc_snoop.Domain;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace dc_snoop.Controllers
{
    [Route("api")]
    public class SnoopController : Controller
    {
        private readonly ISnoopService SnoopService;

        public SnoopController(ISnoopService snoopService)
        {
            this.SnoopService = snoopService;
        }

        [Route("person/{id}")]
        public IActionResult GetPerson(int id)
        {
            return Ok(this.SnoopService.GetPerson(id));
        }

        [Route("address/{id}")]
        public IActionResult GetAddress(int id)
        {
            return Ok(this.SnoopService.GetAddress(id));
        }

        [Route("search")]
        public IActionResult Search(string term)
        {
            if (string.IsNullOrEmpty(term))
            {
                return Ok();
            }
            return Ok(this.SnoopService.Search(term));
        }
    }
}
