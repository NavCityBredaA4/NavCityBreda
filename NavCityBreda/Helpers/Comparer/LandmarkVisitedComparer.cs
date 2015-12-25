using NavCityBreda.Model;
using System.Collections.Generic;

namespace NavCityBreda.Helpers.Comparer
{
    class LandmarkVisitedComparer : IComparer<Landmark>
    {
        public int Compare(Landmark x, Landmark y)
        {
            if (x.Visited == y.Visited)
            {
                return x.Name.CompareTo(y.Name);
            }
            else if (x.Visited)
                return -1;
            else
                return 1;
        }
    }
}
