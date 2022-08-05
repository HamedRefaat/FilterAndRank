using System.Collections.Generic;
using System.Linq;

namespace FilterAndRank
{
  public  class FilterProcessor
    {
        public FilterProcessor()
        {

        }

        public  IEnumerable<long> PeopelWithoutRanking(IList<Person> people, IList<CountryRanking> rankingData)
        {
            var peopleIdes = people.Select(i => i.Id);
            var peopleIdesInRank = rankingData.Select(i => i.PersonId);
            var peopleIdsWithoutRanking = peopleIdes.Except(peopleIdesInRank);
            return peopleIdsWithoutRanking;
        }
        public IEnumerable<long> PeopleNotInCountryFilter(IList<CountryRanking> rankingData, IList<string> countryFilter)
        {
            return rankingData
                            .Where(rd => FilterPeopelByCountryFilter(countryFilter, rd))
                            .Select(rd => rd.PersonId);
        }
        public IEnumerable<long> OutOfRankPeople(IList<CountryRanking> rankingData, int minRank, int maxRank)
        {
            return rankingData
                            .Where(rank => FilterPeopelInRank(minRank, maxRank, rank))
                            .Select(i => i.PersonId);
        }


        private  bool FilterPeopelByCountryFilter(IList<string> countryFilter, CountryRanking countryRanking)
        {
            return !countryFilter.ToLower().Contains(countryRanking.Country.ToLower());
        }
        private  bool FilterPeopelInRank(int minRank, int maxRank, CountryRanking rank)
        {
            return !(rank.Rank >= minRank && rank.Rank <= maxRank);
        }
    }
}
