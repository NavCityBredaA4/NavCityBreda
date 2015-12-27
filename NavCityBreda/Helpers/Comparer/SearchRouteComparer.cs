using NavCityBreda.Model.Search;
using System.Collections.Generic;

namespace NavCityBreda.Helpers.Comparer
{
    class SearchRouteComparer : IComparer<RouteResult>
    {
        public int Compare(RouteResult x, RouteResult y)
        {
            if (x.InDescription == y.InDescription)
                return x.Name.CompareTo(y.Name);
            else if (x.InDescription)
                return 1;
            else
                return -1;
        }
    }
}
