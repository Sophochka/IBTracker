using System;
using System.Collections.Generic;
using IBTracker.Common;

namespace IBTracker.Parsing
{
    public static class NationalRatingFactory
    {
        private static Dictionary<string, INationalRating> raitings = new Dictionary<string, INationalRating>(StringComparer.InvariantCultureIgnoreCase);
        
        static NationalRatingFactory()
        {
            Register(SchoolRegions.EuropeMEastAfrica, SchoolCountries.Poland, new NationalRatingPL());
        }

        public static void Register(string region, string country, INationalRating raiting)
        {
            raitings.Add(GetIndex(region, country), raiting);
        }

        public static bool GetRaiting(string region, string country, out INationalRating raiting)
        {
            return raitings.TryGetValue(GetIndex(region, country), out raiting);
        }

        private static string GetIndex(string region, string country)
        {
            return $"{region}_{country}";
        }
    }
}