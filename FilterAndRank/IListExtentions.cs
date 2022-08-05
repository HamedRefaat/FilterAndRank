using System.Linq;

namespace System.Collections.Generic
{
    public static class ListExtenstions
    {
        public static IList<T> CloneTo<T>(this IList<T> sourec, ref IList<T> destenation)
        {
            destenation = sourec.Select(item => item).ToList();
            return sourec;
        }
        public static IList<T> Limit<T>(this IList<T> sourec, int maxCount)
        {
            return sourec.Skip(0).Take(maxCount).ToList();
        }

        public static IList<string> ToLower(this IList<string> countryFilter)
        {
            return countryFilter.Select(filter => filter.ToLower()).ToList();
        }
    }
}