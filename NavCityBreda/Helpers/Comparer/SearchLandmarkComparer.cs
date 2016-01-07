using NavCityBreda.Model.Search;
using System.Collections.Generic;

namespace NavCityBreda.Helpers.Comparer
{
    class SearchLandmarkComparer : IComparer<LandmarkResult>
    {
        public int Compare(LandmarkResult x, LandmarkResult y)
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
