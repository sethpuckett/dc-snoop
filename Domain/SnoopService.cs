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
            var personMatches = new List<Person>();
            var addressMatches = new List<Address>();
            var addressAggregateMatches = new List<Address>();

            int tempInt;
            var ignoreTerms = new List<string> { "ST", "RD", "CT", "LN", "PL" };
            var termArr = term.ToUpper().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var shortTerms = termArr.Where(t => t.Length <= 2 && !int.TryParse(t, out tempInt) && !ignoreTerms.Contains(t));
            var longTerms = termArr.Where(t => t.Length > 2 || int.TryParse(t, out tempInt));

            foreach (var longTerm in longTerms)
            {
                personMatches = this.Repository.GetAll<Person>()
                    .Where(p => p.FirstName.Contains(longTerm) || p.LastName.Contains(longTerm))
                    .Include(p => p.Address)
                    .ToList();

                var scoreModifier = 2;
                if (personMatches.Count < 50 )
                {
                    scoreModifier = 4;
                }
                else if (personMatches.Count < 200)
                {
                    scoreModifier = 3;
                }
                
                addressMatches = this.Repository.GetAll<Address>()
                    .Where(p => p.StreetName.Contains(longTerm) || p.StreetNumber == longTerm)
                    .Include(a => a.People)
                    .ToList();

                this.CreateOrUpdatePersonMatches(personMatches, addressAggregateMatches, results, scoreModifier);
                this.CreateOrUpdateAddressMatches(addressMatches, addressAggregateMatches, results, 2);
            }

            foreach (var shortTerm in shortTerms)
            {
                addressMatches = addressAggregateMatches
                    .Where(p => p.StreetName == shortTerm).Distinct().ToList();
                this.CreateOrUpdateAddressMatches(addressMatches, addressAggregateMatches, results, 2);  

                addressMatches = addressAggregateMatches
                    .Where(p => p.StreetQuadrant == shortTerm).Distinct().ToList();
                this.CreateOrUpdateAddressMatches(addressMatches, addressAggregateMatches, results, 1);                
            }

            return results
                .Where(r => r.Strength > 3)
                .OrderByDescending(r => r.Strength)
                .ThenByDescending(r => r.Type)
                .ThenBy(r => r.Text);
        }

        private void CreateOrUpdatePersonMatches(IEnumerable<Person> matches, List<Address> aggregateMatches, IList<SearchResult> results, int strengthModifier)
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
                            Strength = strengthModifier
                        });

                    aggregateMatches.Add(personMatch.Address);
                }
                else
                {
                    existingResult.Strength += strengthModifier;
                }
            }
        }

        private void CreateOrUpdateAddressMatches(List<Address> matches, List<Address> aggregateMatches, IList<SearchResult> results, int strengthModifier)
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
                    results.Add(new SearchResult 
                        { 
                            Type = SearchResultType.Address.DisplayName(), 
                            Id = addressMatch.Id, 
                            Text = addressMatch.FullAddress, 
                            Strength = strengthModifier 
                        });

                    aggregateMatches.Add(addressMatch);
                }
                else
                {
                    existingResult.Strength += strengthModifier;
                }

                foreach(var peopleResult in existingPeopleResults)
                {
                    peopleResult.Strength += strengthModifier;
                }
            }
        }
    }
}
