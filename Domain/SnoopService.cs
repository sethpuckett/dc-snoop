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

                this.SearchHelper.UpdatePersonSearchResults(personMatches, results, scoreModifier);
                this.SearchHelper.UpdateAddressSearchResults(addressMatches, results, 2);
                this.SearchHelper.UpdateResidentSearchResults(addressMatches, results, 2);

                allAddressMatches.AddRange(personMatches.Select(p => p.Address));
                allAddressMatches.AddRange(addressMatches);
            }

            // remove duplicate addresses from aggregate list.
            allAddressMatches = allAddressMatches.Distinct().ToList();

            // because short terms can match on so many addresses, only use to modify score from long term matches
            foreach (var shortTerm in shortTerms)
            {
                var addressShortMatches = this.SearchHelper.GetShortTermAddressMatches(allAddressMatches, shortTerm);

                var shortScoreModifier = this.SearchHelper.GetShortTermCountScoreModifier(addressShortMatches.Count);

                this.SearchHelper.UpdateAddressSearchResults(addressShortMatches, results, shortScoreModifier);
                this.SearchHelper.UpdateResidentSearchResults(addressShortMatches, results, 2);
            }

            return results
                .Where(r => r.Strength > 3)
                .OrderByDescending(r => r.Strength)
                .ThenByDescending(r => r.Type)
                .ThenBy(r => r.Text)
                .Take(100);
        }
    }
}
