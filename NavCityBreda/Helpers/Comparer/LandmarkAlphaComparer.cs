using NavCityBreda.Model;
using System.Collections.Generic;

namespace NavCityBreda.Helpers.Comparer
{
    class LandmarkAlphaComparer : IComparer<Landmark>
    {
        public int Compare(Landmark x, Landmark y)
        {
            return x.Name.CompareTo(y.Name);
        }
    }
}
