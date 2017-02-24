using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dc_snoop.DAL;
using dc_snoop.Models;
using dc_snoop.ViewModels;

namespace dc_snoop.Domain
{
    public class SnoopService
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
            return this.Repository.GetById<Person>(id);
        }

        public IEnumerable<Address> GetAllAddresses()
        {
            return this.Repository.GetAll<Address>();
        }

        public Address GetAddress(int id)
        {
            return this.Repository.GetById<Address>(id);
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
                    .ToList();

                var addressMatches = this.Repository.GetAll<Address>()
                    .Where(p => p.Street.ToLower().Contains(singleTerm) || p.Zip.ToLower().Contains(singleTerm));

                this.CreateOrUpdatePersonMatches(personMatches, results);
                this.CreateOrUpdateAddressMatches(addressMatches, results);
            }

            return results;
        }

        public IEnumerable<Person> GetPeopleForAddress(int id)
        {
            return new List<Person>();
        }

        private void CreateOrUpdatePersonMatches(IEnumerable<Person> matches, IList<SearchResult> results)
        {
            foreach (var personMatch in matches)
            {
                var existingResult = results.SingleOrDefault(r => r.Type == SearchResultType.Person && r.Id == personMatch.Id);

                if (existingResult == null)
                {
                    var addressPortion = personMatch.Address != null ? $" - ${personMatch.Address.FullAddress}" : "";

                    results.Add(new SearchResult
                        {
                            Type = SearchResultType.Person,
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

        private void CreateOrUpdateAddressMatches(IEnumerable<Address> matches, IList<SearchResult> results)
        {
            foreach (var addressMatch in matches)
            {
                var existingResult = results.SingleOrDefault(r => r.Type == SearchResultType.Address && r.Id == addressMatch.Id);

                if (existingResult == null)
                {
                    results.Add(new SearchResult { Type = SearchResultType.Address, Id = addressMatch.Id, Text = addressMatch.FullAddress, Strength = 1 });
                }
                else
                {
                    existingResult.Strength++;
                }
            }
        }
    }
}
