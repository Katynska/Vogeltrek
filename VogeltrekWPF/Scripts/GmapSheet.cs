using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using System.Collections.ObjectModel;
using System.Windows;


namespace VogeltrekWPF.Scripts
{
    internal class GmapSheet
    {
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


        public static GMapMarker AddMarker(GMapControl map, double latitude, double longitude, string cityName, bool isPrimaryCity = false)
        {
            // Создаем метку на карту Gmap.NET
            GMapMarker marker = null;
            if (latitude != 0.0 && longitude != 0.0)
            {
                marker = new GMapMarker(new PointLatLng(latitude, longitude))
                {
                    Shape = new TextBlock
                    {
                        Text = cityName,
                        Foreground = isPrimaryCity ? Brushes.Blue : Brushes.Red,
                        FontWeight = FontWeights.Bold,
                        Background = Brushes.White,
                        Padding = new Thickness(5),
                        Opacity = 0.8
                    }
                };

                map.Markers.Add(marker);
            }

            return marker;
        }


        public static void AddRoute(GMapControl map, double startLatitude, double startLongitude, double endLatitude, double endLongitude, string cityName)
        {
            var routePoints = new List<PointLatLng>
    {
        new PointLatLng(startLatitude, startLongitude),
        new PointLatLng(endLatitude, endLongitude)
    };

            // Создаем маршрут
            var route = new GMapRoute(routePoints)
            {
                Shape = new Path
                {
                    Stroke = Brushes.Green,
                    StrokeThickness = 2
                }
            };

            // Добавляем маршрут на карту
            map.Markers.Add(route);

            // Отмечаем город на карте
            var marker = new GMapMarker(new PointLatLng(endLatitude, endLongitude))
            {
                Shape = new TextBlock
                {
                    Text = cityName,
                    Foreground = Brushes.Green,
                    FontWeight = FontWeights.Bold,
                    Background = Brushes.White,
                    Padding = new Thickness(5),
                    Opacity = 0.8
                }
            };

            map.Markers.Add(marker);
        }

        // Метод для удаления всех меток и маршрутов с карты
        public static void ClearMap(GMapControl map)
        {
            map.Markers.Clear();
        }
    }
}
