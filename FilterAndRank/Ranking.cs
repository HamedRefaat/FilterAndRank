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
            FilterProcessor filterProcessor = new();
            return rankingData
                .Filter(filterProcessor.PeopelWithoutRanking(people, rankingData))
                .Filter(filterProcessor.PeopleNotInCountryFilter(rankingData, countryFilter))
                .Filter(filterProcessor.OutOfRankPeople(rankingData, minRank, maxRank))
                .ApplyCustomOrder(people, countryFilter)
                .CloneTo(ref PeopelRankingWithoutLimit)
                .Limit(maxCount)
                .AddSkippedPeopleWithTheSameRankInLimitedPeople(PeopelRankingWithoutLimit.Skip(maxCount))
                .Select(cr => new RankedResult(cr.PersonId, cr.Rank))
                .ToList();
        }

       
    }
}
