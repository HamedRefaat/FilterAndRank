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
            IList<CountryRanking> PeopelRankingWithoutLimit = null;
            return rankingData
                .Filter(PeopelWithoutRanking(people, rankingData))
                .Filter(PeopleNotInCountryFilter(rankingData, countryFilter))
                .Filter(OutOfRankPeople(rankingData, minRank, maxRank))
                .ApplyCustomOrder(people, countryFilter)
                .CloneTo(ref PeopelRankingWithoutLimit)
                .Limit(maxCount)
                .AddSkippedPeopleWithTheSameRankInLimitedPeople(PeopelRankingWithoutLimit.Skip(maxCount))
                .Select(cr => new RankedResult(cr.PersonId, cr.Rank))
                .ToList();
        }

        private static IEnumerable<long> PeopelWithoutRanking(IList<Person> people, IList<CountryRanking> rankingData)
        {
            var peopleIdes = people.Select(i => i.Id);
            var peopleIdesInRank = rankingData.Select(i => i.PersonId);
            var peopleIdsWithoutRanking = peopleIdes.Except(peopleIdesInRank);
            return peopleIdsWithoutRanking;
        }
        private static IEnumerable<long> PeopleNotInCountryFilter(IList<CountryRanking> rankingData, IList<string> countryFilter)
        {
            return rankingData
                            .Where(rd => FilterPeopelByCountryFilter(countryFilter, rd))
                            .Select(rd => rd.PersonId);
        }
        private static IEnumerable<long> OutOfRankPeople(IList<CountryRanking> rankingData, int minRank, int maxRank)
        {
            return rankingData
                            .Where(rank => FilterPeopelInRank(minRank, maxRank, rank))
                            .Select(i => i.PersonId);
        }

      
        private static bool FilterPeopelByCountryFilter(IList<string> countryFilter, CountryRanking countryRanking)
        {
            return !countryFilter.ToLower().Contains(countryRanking.Country.ToLower());
        }
        private static bool FilterPeopelInRank(int minRank, int maxRank, CountryRanking rank)
        {
            return !(rank.Rank >= minRank && rank.Rank <= maxRank);
        }
    }
}
