using System;
using System.Collections.Generic;
using System.Linq;
using dc_snoop.DAL;
using dc_snoop.Models;
using Microsoft.EntityFrameworkCore;

namespace dc_snoop.Domain
{
    public class SearchHelper : ISearchHelper
    {
        private readonly ISnoopRepository Repository;

        private readonly List<string> IgnoredTerms = new List<string> { "ST", "RD", "CT", "LN", "PL" };

        public SearchHelper(ISnoopRepository repository)
        {
            this.Repository = repository;
        }

        public List<string> GetShortSearchTerms(string fullQuery)
        {
            int tempInt;
            var termArr = fullQuery.ToUpper().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return termArr.Where(t => t.Length <= 2 && !int.TryParse(t, out tempInt) && !this.IgnoredTerms.Contains(t)).ToList();
        }

        public List<string> GetLongSearchTerms(string fullQuery)
        {
            int tempInt;
            var termArr = fullQuery.ToUpper().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return termArr.Where(t => t.Length > 2 || int.TryParse(t, out tempInt)).ToList();
        }

        public int GetLongTermCountScoreModifier(int count)
        {
            var scoreModifier = 2;

            if (count < 50 )
            {
                scoreModifier = 4;
            }
            else if (count < 200)
            {
                scoreModifier = 3;
            }
            
            return scoreModifier;
        }

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

        public List<Person> GetLongTermPersonMatches(string term)
        {
            return this.Repository.GetAll<Person>()
                .Where(p => p.FirstName.Contains(term) || p.LastName.Contains(term))
                .Include(p => p.Address)
                .ToList();
        }

        public List<Address> GetLongTermAddressMatches(string term)
        {
            return this.Repository.GetAll<Address>()
                .Where(p => p.StreetName.Contains(term) || p.StreetNumber == term)
                .Include(a => a.People)
                .ToList();
        }

        public List<Address> GetShortTermAddressMatches(IEnumerable<Address> searchList, string term)
        {
            return searchList.Where(p => p.StreetName == term || p.StreetQuadrant == term).Distinct().ToList();
        }
    }
}
