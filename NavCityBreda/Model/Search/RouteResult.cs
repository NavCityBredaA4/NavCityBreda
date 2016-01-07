using NavCityBreda.Model.Object;

namespace NavCityBreda.Model.Search
{
    public class RouteResult
    {
        public Route Route;
        public string Name { get { return Route.Name; } }
        public string Details { get; private set; }
        public bool InDescription { get; private set; }

        public RouteResult(Route r, string searchdetail, bool indesc)
        {
            Route = r;
            Details = searchdetail;
            InDescription = indesc;
        }
    }
}
