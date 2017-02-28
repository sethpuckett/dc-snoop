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

        private readonly ISearchHelper SearchHelper;

        public SnoopService(ISnoopRepository repository, ISearchHelper searchHelper)
        {
            this.Repository = repository;
            this.SearchHelper = searchHelper;
        }

        public Person GetPerson(int id)
        {
            return this.Repository.GetById<Person>(id)
                .Include(p => p.Address)
                .FirstOrDefault();
        }

        public Address GetAddress(int id)
        {
            return this.Repository.GetById<Address>(id)
                .Include(a => a.People).FirstOrDefault();
        }

        public IEnumerable<SearchResult> Search(string term)
        {
            var results = new List<SearchResult>();
            var allAddressMatches = new List<Address>();

            // Separate long terms (full names, street numbers) from short terms(street letters, quadrants)
            var shortTerms = this.SearchHelper.GetShortSearchTerms(term);
            var longTerms = this.SearchHelper.GetLongSearchTerms(term);

            // search all people and address for long term matches
            foreach (var longTerm in longTerms)
            {
                var personMatches = this.SearchHelper.GetLongTermPersonMatches(longTerm);
                var addressMatches = this.SearchHelper.GetLongTermAddressMatches(longTerm);

                var scoreModifier = this.SearchHelper.GetLongTermCountScoreModifier(personMatches.Count);                

                this.CreateOrUpdatePersonMatches(personMatches, allAddressMatches, results, scoreModifier);
                this.CreateOrUpdateAddressMatches(addressMatches, allAddressMatches, results, 2);
            }

            // because short terms can match on so many addresses, only use to modify score from long term matches
            foreach (var shortTerm in shortTerms)
            {
                var addressShortMatches = this.SearchHelper.GetShortTermAddressMatches(allAddressMatches, shortTerm);

                var shortScoreModifier = this.SearchHelper.GetShortTermCountScoreModifier(addressShortMatches.Count);

                this.CreateOrUpdateAddressMatches(addressShortMatches, allAddressMatches, results, shortScoreModifier);  
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
