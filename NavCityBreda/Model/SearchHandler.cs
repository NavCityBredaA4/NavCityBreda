using NavCityBreda.Helpers;
using NavCityBreda.Helpers.Comparer;
using NavCityBreda.Model.Object;
using NavCityBreda.Model.Search;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NavCityBreda.Model
{
    public class SearchHandler
    {
        public static async Task<SearchResult> Search(string term, bool description)
        {
            SearchResult rs = await SearchForResults(term, description);

            return rs;
        }

        private static async Task<SearchResult> SearchForResults(string term, bool description)
        {
            List<RouteResult> rresults = new List<RouteResult>();
            List<LandmarkResult> lresults = new List<LandmarkResult>();

            SearchResult result = new SearchResult(term);

            await Task.Run(() =>
            {
                //Search routes
                foreach(Route r in App.RouteManager.Routes)
                {
                    bool routefound = false;
                    int landmarksinroutefound = 0;
                    string landmarks = "";

                    //Search landmarks
                    foreach (Landmark l in r.Landmarks)
                    {
                        if (l.Name.ToLower().Contains(term.ToLower()))
                        {
                            routefound = true;
                            landmarks += (landmarksinroutefound > 0 ? ", " : "") + l.Name;
                            landmarksinroutefound++;

                            LandmarkResult ls = new LandmarkResult(l, "", false);
                            lresults.Add(ls);
                        }
                        else if (description)
                        {
                            if (l.Description.ToLower().Contains(term.ToLower()))
                            {
                                LandmarkResult ls = new LandmarkResult(l, Util.Loader.GetString("SearchResultDescription"), true);
                                lresults.Add(ls);
                            }
                        } 
                    }

                    if(routefound)
                    {
                        string desc = "";
                        if (landmarksinroutefound <= 3)
                            desc = landmarks;
                        else
                            desc = landmarksinroutefound + " " + Util.Loader.GetString("Landmarks"); 

                        RouteResult rs = new RouteResult(r, desc, true);
                        rresults.Add(rs);
                    }
                    else if (r.Name.ToLower().Contains(term.ToLower()))
                    {
                        RouteResult rs = new RouteResult(r, "", false);
                        rresults.Add(rs);
                    }

                    else if(description)
                    {
                        if (r.Description.ToLower().Contains(term.ToLower()))
                        {
                            RouteResult rs = new RouteResult(r, Util.Loader.GetString("SearchResultDescription"), true);
                            rresults.Add(rs);
                        }
                    }

                    
                }
            });

            rresults.Sort(new SearchRouteComparer());
            lresults.Sort(new SearchLandmarkComparer());

            result.Add(rresults);
            result.Add(lresults);

            return result;
        }
    }
}
