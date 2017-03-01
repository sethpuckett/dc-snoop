using System.Collections.Generic;
using dc_snoop.Models;
using dc_snoop.ViewModels;

namespace dc_snoop.Domain
{
    public interface ISearchHelper
    {
        List<string> GetShortSearchTerms(string fullQuery);

        List<string> GetLongSearchTerms(string fullQuery);

        int GetLongTermCountScoreModifier(int count);

        int GetShortTermCountScoreModifier(int count);

        List<Person> GetLongTermPersonMatches(string term);

        List<Address> GetLongTermAddressMatches(string term);

        List<Address> GetShortTermAddressMatches(IEnumerable<Address> searchList, string term);

        void UpdatePersonSearchResults(IEnumerable<Person> matches, IDictionary<string, SearchResult> searchResults, int strengthModifier);

        void UpdateAddressSearchResults(List<Address> matches, IDictionary<string, SearchResult> searchResults, int strengthModifier);

        void UpdateResidentSearchResults(List<Address> matches, IDictionary<string, SearchResult> searchResults, int strengthModifier);
    }
}
