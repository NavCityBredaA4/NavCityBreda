using NavCityBreda.Model.Object;
using System.Collections.Generic;

namespace NavCityBreda.Helpers.Comparer
{
    class LandmarkNotVisitedComparer : IComparer<Landmark>
    {
        public int Compare(Landmark x, Landmark y)
        {
            if (x.Status == y.Status)
                return x.Name.CompareTo(y.Name);
            else if (x.Status == Landmark.LandmarkStatus.VISITED)
                return 1;
            else if (y.Status == Landmark.LandmarkStatus.VISITED)
                return -1;
            else if (x.Status == Landmark.LandmarkStatus.SKIPPED)
                return 1;
            else if (y.Status == Landmark.LandmarkStatus.SKIPPED)
                return -1;
            else
                return 1;
        }
    }
}
