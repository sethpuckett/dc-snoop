using System;
using System.Collections.Generic;
using System.Linq;
using dc_snoop.DAL;
using dc_snoop.Models;
using dc_snoop.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace dc_snoop.Domain
{
    public class SearchHelper : ISearchHelper
    {
        private readonly ISnoopRepository Repository;

        // ignore common road types
        private readonly List<string> IgnoredTerms = new List<string> { "ST", "RD", "CT", "LN", "PL" };

        public SearchHelper(ISnoopRepository repository)
        {
            this.Repository = repository;
        }

        // separate and return the short search terms from a query string
        public List<string> GetShortSearchTerms(string fullQuery)
        {
            int tempInt;
            var termArr = fullQuery.ToUpper().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return termArr.Where(t => t.Length <= 2 && !int.TryParse(t, out tempInt) && !this.IgnoredTerms.Contains(t)).ToList();
        }

        // separate and return the long search terms from a query string
        public List<string> GetLongSearchTerms(string fullQuery)
        {
            int tempInt;
            var termArr = fullQuery.ToUpper().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return termArr.Where(t => t.Length > 2 || int.TryParse(t, out tempInt)).ToList();
        }

        // Return a score modifier for long terms based on the number of matches
        public int GetLongTermCountScoreModifier(int count)
        {
            var scoreModifier = 2;

            if (count < 25 )
            {
                scoreModifier = 4;
            }
            else if (count < 100)
            {
                scoreModifier = 3;
            }
            
            return scoreModifier;
        }

        // Return a score modifier for short terms based on the number of matches
        public int GetShortTermCountScoreModifier(int count)
        {
            var scoreModifier = 1;

            if (count < 5 )
            {
                scoreModifier = 3;
            }
            else if (count < 25)
            {
                scoreModifier = 2;
            }
            
            return scoreModifier;
        }

        // search the repository for people matching the term
        public List<Person> GetLongTermPersonMatches(string term)
        {
            return this.Repository.GetAll<Person>()
                .Where(p => p.FirstName.Contains(term) || p.LastName.Contains(term))
                .Include(p => p.Address)
                .ToList();
        }

        // search the repository for addresses matching the term
        public List<Address> GetLongTermAddressMatches(string term)
        {
            return this.Repository.GetAll<Address>()
                .Where(p => p.StreetName.Contains(term) || p.StreetNumber == term)
                .Include(a => a.People)
                .ToList();
        }

        // search existing matches for addresses matching the term
        public List<Address> GetShortTermAddressMatches(IEnumerable<Address> searchList, string term)
        {
            return searchList.Where(p => p.StreetName == term || p.StreetQuadrant == term).Distinct().ToList();
        }

        // Create search result for each person, or increase score if already matched on previous term
        public void UpdatePersonSearchResults(IEnumerable<Person> matches, IDictionary<string, SearchResult> searchResults, int strengthModifier)
        {
            foreach (var personMatch in matches)
            {
                SearchResult existingResult;
                var key = $"p{personMatch.Id}";
                
                if (searchResults.TryGetValue(key, out existingResult))
                {
                    existingResult.Strength += strengthModifier;
                }
                else
                {
                    var result = this.CreatePersonSearchResult(personMatch, strengthModifier);
                    searchResults.Add(key, result);
                }
            }
        }

        // Create search result for each address, or increase score if already matched on previous term
        public void UpdateAddressSearchResults(List<Address> matches, IDictionary<string, SearchResult> searchResults, int strengthModifier)
        {
            foreach (var addressMatch in matches)
            {
                SearchResult existingResult;
                var key = $"a{addressMatch.Id}";

                if (searchResults.TryGetValue(key, out existingResult))
                {
                    existingResult.Strength += strengthModifier;
                }
                else
                {
                    var result = this.CreateAddressSearchResult(addressMatch, strengthModifier);
                    searchResults.Add(key, result);
                }
            }
        }

        // Increase score for person that is a resident of an address if they already matched on another term
        public void UpdateResidentSearchResults(List<Address> matches, IDictionary<string, SearchResult> searchResults, int strengthModifier)
        {
            foreach (var addressMatch in matches)
            {
                var keys = addressMatch.People.Select(p => $"p{p.Id}");

                foreach(var key in keys)
                {
                    SearchResult existingResult;
                    if (searchResults.TryGetValue(key, out existingResult))
                    {
                        existingResult.Strength += strengthModifier;
                    }
                }
            }
        }

        private SearchResult CreatePersonSearchResult(Person person, int strength)
        {
            return new SearchResult
                {
                    Type = SearchResultType.Person.DisplayName(),
                    Id = person.Id,
                    Text = this.GetPersonSearchResultText(person),
                    Strength = strength
                };
        }

        private SearchResult CreateAddressSearchResult(Address address, int strength)
        {
            return new SearchResult 
                { 
                    Type = SearchResultType.Address.DisplayName(), 
                    Id = address.Id, 
                    Text = address.FullAddress, 
                    Strength = strength 
                };
        }

        private string GetPersonSearchResultText(Person person)
        {
            var addressPortion = person.Address != null ? $" - {person.Address.FullAddress}" : "";
            return $"{person.FullName}{addressPortion}";
        }
    }
}
