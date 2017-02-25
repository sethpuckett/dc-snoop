using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dc_snoop.Models
{
    public class Person : IEntity
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string MiddleName { get; set; }

        public DateTime? RegistrationDate { get; set; }

        public string Affiliation { get; set; }

        public Address Address { get; set; }

        public string Unit { get; set; }

        public string Status1504 { get; set; }

        public string Status1411 { get; set; }

        public string Status1407 { get; set; }

        public string Status1404 { get; set; }

        public string Status1304 { get; set; }

        public string Status1211 { get; set; }

        public string FullName
        {
            get
            {
                return $"{this.FirstName} {this.LastName}";
            }
        }
    }
}
