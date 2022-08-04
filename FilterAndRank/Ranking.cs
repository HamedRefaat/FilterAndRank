using System;
using System.Collections.Generic;
using System.Linq;

namespace FilterAndRank
{
    public static class Ranking
    {        
        public static IList<RankedResult> FilterByCountryWithRank(
            IList<Person> people,
            IList<CountryRanking> rankingData,
            IList<string> countryFilter,
            int minRank, int maxRank,
            int maxCount)
        {
            // TODO: write your solution here.  Do not change the method signature in any way, or validation of your solution will fail.

            var filterdPeopelRanking = rankingData
                .Filter(PeopelWithoutRanking(people, rankingData))
                .Filter(PeopleNotInCountryFilter(rankingData, countryFilter))
                .Filter(OutOfRankPeople(rankingData, minRank, maxRank))
                .ApplyCustomFilter(people, countryFilter);

           

            var orderdPeopleRankingLimit = filterdPeopelRanking.Skip(0).Take(maxCount);

            int lastRank = orderdPeopleRankingLimit.Select(cr => cr.Rank).LastOrDefault();

            var exceedLimitPeopleRanking = filterdPeopelRanking.Skip(maxCount);

            var peopleWithTheSameRank = exceedLimitPeopleRanking.Where(cRank => cRank.Rank == lastRank)
               .Select(p => new RankedResult(p.PersonId, p.Rank))
               .ToList();

            var filterByCountryWithRankPeople = orderdPeopleRankingLimit
                .Select(p => new RankedResult(p.PersonId, p.Rank))
                .ToList();
            filterByCountryWithRankPeople.AddRange(peopleWithTheSameRank);

            return filterByCountryWithRankPeople;
        }

        private static int PeopleSortedByNameCaseInsensetive(IList<Person> people, CountryRanking countryRanking, List<string> sortedFilterdPeopelPeople)
        {
            return sortedFilterdPeopelPeople.IndexOf(people.FirstOrDefault(p => p.Id == countryRanking.PersonId).Name);
        }

        private static int CountryAsSortedInCountryFilter(IList<string> countryFilter, CountryRanking i)
        {
            return countryFilter.IndexOf(i.Country.ToLower());
        }

        private static IEnumerable<long> OutOfRankPeople(IList<CountryRanking> rankingData, int minRank, int maxRank)
        {
            return rankingData
                            .Where(rank => FilterPeopelInRank(minRank, maxRank, rank))
                            .Select(i => i.PersonId);
        }

        private static IEnumerable<long> PeopleNotInCountryFilter(IList<CountryRanking> rankingData, IList<string> countryFilter)
        {
            return rankingData
                            .Where(rd => FilterPeopelByCountryFilter(countryFilter, rd))
                            .Select(rd => rd.PersonId);
        }

        private static IEnumerable<long> PeopelWithoutRanking(IList<Person> people, IList<CountryRanking> rankingData)
        {
            var peopleIdes = people.Select(i => i.Id);
            var peopleIdesInRank = rankingData.Select(i => i.PersonId);
            var peopleIdsWithoutRanking = peopleIdes.Except(peopleIdesInRank);
            return peopleIdsWithoutRanking;
        }

        private static bool FilterPeopelInRank(int minRank, int maxRank, CountryRanking rank)
        {
            return !(rank.Rank >= minRank && rank.Rank <= maxRank);
        }

        private static bool FilterPeopelByCountryFilter(IList<string> countryFilter, CountryRanking countryRanking)
        {
            return !countryFilter.ToLower().Contains(countryRanking.Country.ToLower());
        }

        private static IList<string> ToLower(this IList<string> countryFilter)
        {
            return countryFilter.Select(filter => filter.ToLower()).ToList();
        }

        private static IList<CountryRanking> Filter(this IList<CountryRanking> peopleCountryRank, IEnumerable<long> peopleIds)
        {
            return peopleCountryRank.Where(i => !peopleIds.Contains(i.PersonId)).ToList();
        }

        private static IList<CountryRanking> ApplyCustomFilter(this IList<CountryRanking> filterdPeopelRanking, IList<Person> people, IList<string> countryFilter)
        {
            var sortedPeopleNameCaseInSensetive = people.Where(p => filterdPeopelRanking.Select(i => i.PersonId).Contains(p.Id)).Select(p => p.Name).ToList();
            sortedPeopleNameCaseInSensetive.Sort(StringComparer.InvariantCultureIgnoreCase);

            return filterdPeopelRanking
                .OrderBy(i => i.Rank)
                .ThenBy(i => CountryAsSortedInCountryFilter(countryFilter, i))
                .ThenBy(i => PeopleSortedByNameCaseInsensetive(people, i, sortedPeopleNameCaseInSensetive)).ToList();
        }
    }
}
