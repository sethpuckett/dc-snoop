using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dc_snoop.DAL;
using dc_snoop.Models;
using dc_snoop.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace dc_snoop.Domain
{
    public class SnoopService : ISnoopService
    {
        private readonly ISnoopRepository Repository;

        public SnoopService(ISnoopRepository repository)
        {
            this.Repository = repository;
        }

        public IEnumerable<Person> GetAllPeople()
        {
            return this.Repository.GetAll<Person>();
        }

        public Person GetPerson(int id)
        {
            return this.Repository.GetById<Person>(id)
                .Include(p => p.Address)
                .FirstOrDefault();
        }

        public IEnumerable<Address> GetAllAddresses()
        {
            return this.Repository.GetAll<Address>();
        }

        public Address GetAddress(int id)
        {
            return this.Repository.GetById<Address>(id)
                .Include(a => a.People).FirstOrDefault();
        }

        public IEnumerable<SearchResult> Search(string term)
        {
            var results = new List<SearchResult>();

            var termArr = term.ToLower().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            // remove short strings (except numbers)
            int tempInt;
            var filteredTermList = termArr.Where(t => t.Length > 2 || int.TryParse(t, out tempInt));

            foreach (var singleTerm in filteredTermList)
            {
                var personMatches = this.Repository.GetAll<Person>()
                    .Where(p => p.FirstName.ToLower().Contains(singleTerm) || p.LastName.ToLower().Contains(singleTerm))
                    .Include(p => p.Address)
                    .ToList();

                var addressMatches = this.Repository.GetAll<Address>()
                    .Where(p => p.Street.ToLower().Contains(singleTerm) || p.Zip.ToLower().Contains(singleTerm))
                    .Include(a => a.People)
                    .ToList();

                this.CreateOrUpdatePersonMatches(personMatches, results);
                this.CreateOrUpdateAddressMatches(addressMatches, personMatches, results);
            }

            return results.OrderByDescending(r => r.Strength).ThenByDescending(r => r.Type).ThenBy(r => r.Text);
        }

        private void CreateOrUpdatePersonMatches(IEnumerable<Person> matches, IList<SearchResult> results)
        {
            foreach (var personMatch in matches)
            {
                var existingResult = results.SingleOrDefault(r => r.Type == SearchResultType.Person.DisplayName() && r.Id == personMatch.Id);

                if (existingResult == null)
                {
                    var addressPortion = personMatch.Address != null ? $" - {personMatch.Address.FullAddress}" : "";

                    results.Add(new SearchResult
                        {
                            Type = SearchResultType.Person.DisplayName(),
                            Id = personMatch.Id,
                            Text = $"{personMatch.FullName}{addressPortion}",
                            Strength = 1
                        });
                }
                else
                {
                    existingResult.Strength++;
                }
            }
        }

        private void CreateOrUpdateAddressMatches(IEnumerable<Address> matches, IEnumerable<Person> personMatches, IList<SearchResult> results)
        {
            foreach (var addressMatch in matches)
            {
                var existingResult = results
                    .SingleOrDefault(r => r.Type == SearchResultType.Address.DisplayName() 
                        && r.Id == addressMatch.Id);

                var existingPeopleResults = results
                    .Where(r => r.Type == SearchResultType.Person.DisplayName() 
                        && addressMatch.People.Select(p => p.Id).Contains(r.Id));

                if (existingResult == null)
                {
                    results.Add(new SearchResult { Type = SearchResultType.Address.DisplayName(), Id = addressMatch.Id, Text = addressMatch.FullAddress, Strength = 1 });
                }
                else
                {
                    existingResult.Strength++;
                }

                foreach(var peopleResult in existingPeopleResults)
                {
                    peopleResult.Strength++;
                }
            }
        }
    }
}
