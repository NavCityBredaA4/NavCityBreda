using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;

namespace NavCityBreda.Model
{
    public class CompassTracker
    {
        public delegate void OnHeadingUpdatedHandler(object sender, HeadingUpdatedEventArgs e);
        public event OnHeadingUpdatedHandler OnHeadingUpdated;

        private Compass comp;

        private string status;
        public string Status { get { return status; } }

        private CompassReading hdn;
        public CompassReading Heading { get { return hdn; } }

        public CompassTracker()
        {
            status = "Waiting...";
            comp = Compass.GetDefault();

            // Assign an event handler for the compass reading-changed event
            if (comp != null)
            {
                // Establish the report interval for all scenarios
                uint minReportInterval = comp.MinimumReportInterval;
                uint reportInterval = minReportInterval > 16 ? minReportInterval : 16;
                comp.ReportInterval = reportInterval;
                comp.ReadingChanged += Comp_ReadingChanged;
            }
        }

        private void Comp_ReadingChanged(Compass sender, CompassReadingChangedEventArgs args)
        {
            UpdateHeading(args.Reading);
        }

        private void UpdateHeading(CompassReading r)
        {
            hdn = r;

            //Make sure someone is listening
            if (OnHeadingUpdated == null) return;

            OnHeadingUpdated(this, new HeadingUpdatedEventArgs(r));
        }
    }

    public class HeadingUpdatedEventArgs : EventArgs
    {
        public CompassReading Heading;

        public HeadingUpdatedEventArgs(CompassReading heading)
        {
            Heading = heading;
        }
    }
}
