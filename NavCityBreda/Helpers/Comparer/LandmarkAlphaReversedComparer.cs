using NavCityBreda.Model.Object;
using System.Collections.Generic;

namespace NavCityBreda.Helpers.Comparer
{
    class LandmarkAlphaReversedComparer : IComparer<Landmark>
    {
        public int Compare(Landmark x, Landmark y)
        {
            return y.Name.CompareTo(x.Name);
        }
    }
}
