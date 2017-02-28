using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace dc_snoop.Models
{
    public class Address : IEntity
    {
        public int Id { get; set; }
        
        public string Street { get; set; }

        public string StreetNumber { get; set; }

        public string StreetName { get; set; }

        public string StreetType { get; set; }

        public string StreetQuadrant { get; set; }

        public string Zip { get; set; }

        public string Precinct { get; set; }

        public string Ward { get; set; }

        public virtual ICollection<Person> People { get; set; } 

        public string FullAddress
        {
            get
            {
                return $"{this.Street}, {this.Zip}";
            }
        }
    }
}
