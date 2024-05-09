using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;


namespace VogeltrekWPF.Scripts
{
    internal class GmapSheet
    {
        private static GMapMarker previousMarker = null;

        public static void ConfigureMap(GMapControl map)
        {
            GMaps.Instance.Mode = AccessMode.ServerAndCache;
            // choose your provider here
            map.MapProvider = GMap.NET.MapProviders.OpenStreetMapProvider.Instance;
            map.MinZoom = 2;
            map.MaxZoom = 17;
            // whole world zoom
            map.Zoom = 2;
            // lets the map use the mousewheel to zoom
            map.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            // lets the user drag the map
            map.CanDragMap = true;
            // lets the user drag the map with the left mouse button
            map.DragButton = MouseButton.Left;
        }



        public static void AddMarker(GMapControl map, double latitude, double longitude)
        {
            // Удаление предыдущей метки, если она существует
            if (previousMarker != null)
            {
                map.Markers.Remove(previousMarker);
            }

            // Добавляем метку на карту Gmap.NET
            if (latitude != 0.0 && longitude != 0.0)
            {
                GMapMarker marker = new GMapMarker(new PointLatLng(latitude, longitude));
                {
                    marker.Shape = new Ellipse
                    {
                        Width = 10,
                        Height = 10,
                        Stroke = Brushes.Red,
                        Fill = Brushes.Red,
                        Opacity = 0.6
                    };
                }
                map.Markers.Add(marker);

                // Сохраняем ссылку на текущую метку как предыдущую
                previousMarker = marker;
            }
        }
    }
}
