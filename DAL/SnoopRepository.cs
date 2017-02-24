using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dc_snoop.DAL
{
    public class SnoopRepository : ISnoopRepository
    {
        private readonly SnoopContext context;

        public SnoopRepository(SnoopContext context)
        {
            this.context = context;
        }
    }
}
