using System;
using System.Collections.Generic;
using System.Linq;

namespace FilterAndRank
{
    public static class CountryRankingExtention
    {
      
        public static IList<CountryRanking> AddSkippedPeopleWithTheSameRankInLimitedPeople(this IList<CountryRanking> SelectedPeople, IEnumerable<CountryRanking> skippedPeople)
        {

            int lastRank = SelectedPeople.Select(cr => cr.Rank).LastOrDefault();
            skippedPeople
               .Where(cRank => cRank.Rank == lastRank)
               .ToList()
               .ForEach(skiped =>
               {
                   SelectedPeople.Add(skiped);
               });

            return SelectedPeople;


        }

        public static IList<CountryRanking> ApplyCustomOrder(this IList<CountryRanking> filterdPeopelRanking, IList<Person> people, IList<string> countryFilter)
        {
            var sortedPeopleNameCaseInSensetive = people.Where(p => filterdPeopelRanking.Select(i => i.PersonId).Contains(p.Id)).Select(p => p.Name).ToList();
            sortedPeopleNameCaseInSensetive.Sort(StringComparer.InvariantCultureIgnoreCase);

            return filterdPeopelRanking
                .OrderBy(i => i.Rank)
                .ThenBy(i => CountryAsSortedInCountryFilter(countryFilter, i))
                .ThenBy(i => PeopleSortedByNameCaseInsensetive(people, i, sortedPeopleNameCaseInSensetive)).ToList();
        }

        public static IList<CountryRanking> Filter(this IList<CountryRanking> peopleCountryRank, IEnumerable<long> peopleIds)
        {
            return peopleCountryRank.Where(i => !peopleIds.Contains(i.PersonId)).ToList();
        }
     
       

      

        private static int PeopleSortedByNameCaseInsensetive(IList<Person> people, CountryRanking countryRanking, List<string> sortedFilterdPeopelPeople)
        {
            return sortedFilterdPeopelPeople.IndexOf(people.FirstOrDefault(p => p.Id == countryRanking.PersonId).Name);
        }
        private static int CountryAsSortedInCountryFilter(IList<string> countryFilter, CountryRanking i)
        {
            return countryFilter.IndexOf(i.Country.ToLower());
        }
    }
}