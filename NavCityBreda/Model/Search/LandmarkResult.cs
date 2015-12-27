using NavCityBreda.Model.Object;

namespace NavCityBreda.Model.Search
{
    public class LandmarkResult
    {
        public Landmark Landmark;
        public string Name { get { return Landmark.Name; } }
        public string Details { get; private set; }
        public bool InDescription { get; private set; }

        public LandmarkResult(Landmark l, string searchdetail, bool indesc)
        {
            Landmark = l;
            Details = searchdetail;
            InDescription = indesc;
        }
    }
}
