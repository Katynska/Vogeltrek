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
using System.Windows.Media.Imaging;


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
            map.Zoom = 3; // Устанавливаем начальный масштаб
            map.Position = new PointLatLng(64.6863, 97.7453); // Устанавливаем начальные координаты центра (примерно центральная точка России)
            // lets the map use the mousewheel to zoom
            map.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            // lets the user drag the map
            map.CanDragMap = true;
            // lets the user drag the map with the left mouse button
            map.DragButton = MouseButton.Left;
        }


        //Метод добовления названий городов на карту при выборе из рейтинга или выподающего списка
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


        //Метод орисовки линий к городам после завершения опроса
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


        //Метод отрисовки Вкл/Выкл отображения всех городов
        public static void AddCircle(GMapControl map, double latitude, double longitude, string cityName, Brush color)
        {
            // Создаем кружок на карту Gmap.NET
            Ellipse circle = new Ellipse()
            {
                Width = 10,
                Height = 10,
                Fill = color, // Цвет кружка
                Opacity = 0.8
            };

            // При наведении мыши на кружок показываем название города
            circle.ToolTip = cityName;

            // Устанавливаем координаты кружка
            GMapMarker marker = new GMapMarker(new PointLatLng(latitude, longitude));
            marker.Shape = circle;

            // Добавляем кружок на карту
            map.Markers.Add(marker);
        }


        // Метод для удаления всех меток и маршрутов с карты
        public static void ClearMap(GMapControl map)
        {
            map.Markers.Clear();
        }

        //Метод сохранения скриншота карты
        public static void SaveMapAsImage(GMapControl map)
        {
            RenderTargetBitmap renderTarget = new RenderTargetBitmap((int)map.ActualWidth, (int)map.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            renderTarget.Render(map);

            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderTarget));

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string filePath = $"MapScreen_{timestamp}.png"; // уникальный путь к файлу для сохранения
            using (var fileStream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
            {
                encoder.Save(fileStream);
            }

            MessageBox.Show("Скриншот карты сохранен как " + filePath);
        }


        // Метод для возврата карты в исходное положение
        public static void SetDefaultView(GMapControl map)
        {
            map.Position = new PointLatLng(64.6863, 97.7453); // Устанавливаем начальные координаты центра
            map.Zoom = 3; // Устанавливаем начальный масштаб
        }

}
    }

