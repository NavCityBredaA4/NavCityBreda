using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace NavCityBreda.Helpers
{
    public class BoundingBox
    {
        public static GeoboundingBox GetBoundingBox(List<Geopoint> points)
        {
            double latitude_min = points.Select(p => p.Position.Latitude).Min();
            double latitude_max = points.Select(p => p.Position.Latitude).Max();
            double longitude_min = points.Select(p => p.Position.Longitude).Min();
            double longitude_max = points.Select(p => p.Position.Longitude).Max();

            return new GeoboundingBox(
                new BasicGeoposition { Latitude = latitude_max, Longitude = longitude_max },
                new BasicGeoposition { Latitude = latitude_min, Longitude = longitude_min }
            );
        }

        // Semi-axes of WGS-84 geoidal reference
        private const double WGS84_a = 6378137.0; // Major semiaxis [m]
        private const double WGS84_b = 6356752.3; // Minor semiaxis [m]

        // 'halfSideInKm' is the half length of the bounding box you want in kilometers.
        public static GeoboundingBox GetBoundingBox(Geopoint point, double halfSideInKm)
        {
            // Bounding box surrounding the point at given coordinates,
            // assuming local approximation of Earth surface as a sphere
            // of radius given by WGS84
            var lat = Deg2rad(point.Position.Latitude);
            var lon = Deg2rad(point.Position.Longitude);
            var halfSide = 1000 * halfSideInKm;

            // Radius of Earth at given latitude
            var radius = WGS84EarthRadius(lat);
            // Radius of the parallel at given latitude
            var pradius = radius * Math.Cos(lat);

            var latMin = lat - halfSide / radius;
            var latMax = lat + halfSide / radius;
            var lonMin = lon - halfSide / pradius;
            var lonMax = lon + halfSide / pradius;

            return new GeoboundingBox(
                new BasicGeoposition { Latitude = Rad2deg(latMax), Longitude = Rad2deg(lonMax) },
                new BasicGeoposition { Latitude = Rad2deg(latMin), Longitude = Rad2deg(lonMin) }
                
             );
        }

        // degrees to radians
        private static double Deg2rad(double degrees)
        {
            return Math.PI * degrees / 180.0;
        }

        // radians to degrees
        private static double Rad2deg(double radians)
        {
            return 180.0 * radians / Math.PI;
        }

        // Earth radius at a given latitude, according to the WGS-84 ellipsoid [m]
        private static double WGS84EarthRadius(double lat)
        {
            var An = WGS84_a * WGS84_a * Math.Cos(lat);
            var Bn = WGS84_b * WGS84_b * Math.Sin(lat);
            var Ad = WGS84_a * Math.Cos(lat);
            var Bd = WGS84_b * Math.Sin(lat);
            return Math.Sqrt((An * An + Bn * Bn) / (Ad * Ad + Bd * Bd));
        }
    }

}
