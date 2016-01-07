using System.Collections.Generic;

namespace NavCityBreda.Model.Search
{
    public class SearchResult
    {
        public List<RouteResult> Routes { get; private set; }
        public List<LandmarkResult> Landmarks { get; private set; }

        public string SearchTerm { get; private set; }
        public int Results { get { return (Routes.Count + Landmarks.Count); } }

        public SearchResult(string term)
        {
            SearchTerm = term;
            Routes = new List<RouteResult>();
            Landmarks = new List<LandmarkResult>();
        }

        public void Add(RouteResult r)
        {
            Routes.Add(r);
        }

        public void Add(List<RouteResult> r)
        {
            Routes.AddRange(r);
        }

        public void Add(LandmarkResult l)
        {
            Landmarks.Add(l);
        }

        public void Add(List<LandmarkResult> l)
        {
            Landmarks.AddRange(l);
        }
    }
}
