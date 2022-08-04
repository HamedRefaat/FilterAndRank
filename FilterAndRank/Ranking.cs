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

            var peopleIdes = people.Select(i => i.Id);
            var peopleIdesInRank = rankingData.Select(i => i.PersonId);
            var peopleIdsWithoutRanking = peopleIdes.Except(peopleIdesInRank);

            var IgnoredPeopleIds = new List<long>();
            IgnoredPeopleIds.AddRange(peopleIdsWithoutRanking);

            var NotInCountryFilterPeopleIds = rankingData
                .Where(rd => FilterPeopelByCountryFilter(countryFilter, rd))
                .Select(rd => rd.PersonId);

            IgnoredPeopleIds.AddRange(NotInCountryFilterPeopleIds);

            var outOfRankPeople = rankingData
                .Where(rank => FilterPeopelInRank(minRank, maxRank, rank))
                .Select(i => i.PersonId);

            IgnoredPeopleIds.AddRange(outOfRankPeople);

            var remaingPeopleRanking = rankingData
                .Where(i => !IgnoredPeopleIds.Contains(i.PersonId));

            var sortedFilterdPeopelPeople = people.Where(p => !IgnoredPeopleIds.Contains(p.Id)).Select(p => p.Name).ToList();
            sortedFilterdPeopelPeople.Sort(StringComparer.InvariantCultureIgnoreCase);

            var orderdPeopleRanking = remaingPeopleRanking
                .OrderBy(i => i.Rank)
                .ThenBy(i => countryFilter.IndexOf(i.Country.ToLower()))
                .ThenBy(i => sortedFilterdPeopelPeople.IndexOf(people.FirstOrDefault(p => p.Id == i.PersonId).Name));

            var orderdMaxPeopleRanking = orderdPeopleRanking.Skip(0).Take(maxCount);

            int lastRank = orderdMaxPeopleRanking.Select(pR => pR.Rank).LastOrDefault();

            var afterMaxCountPeople = orderdPeopleRanking.Skip(maxCount);

            var peopleWithTheSameRank = afterMaxCountPeople.Where(cRank => cRank.Rank == lastRank)
               .Select(p => new RankedResult(p.PersonId, p.Rank))
               .ToList();

            var filterdPeople = orderdMaxPeopleRanking
                .Select(p => new RankedResult(p.PersonId, p.Rank))
                .ToList();
            filterdPeople.AddRange(peopleWithTheSameRank);

            return filterdPeople;
        }

        private static bool FilterPeopelInRank(int minRank, int maxRank, CountryRanking rank)
        {
            return !(rank.Rank >= minRank && rank.Rank <= maxRank);
        }

        private static bool FilterPeopelByCountryFilter(IList<string> countryFilter, CountryRanking countryRanking)
        {
            return !countryFilter.Select(filter => filter.ToLower()).Contains(countryRanking.Country.ToLower());
        }
    }
}
