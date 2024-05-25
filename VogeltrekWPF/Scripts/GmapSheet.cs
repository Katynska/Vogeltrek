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
            // Устанавливаем режим доступа к данным (с сервера и из кэша)
            GMaps.Instance.Mode = AccessMode.ServerAndCache;
            // Выбираем провайдера карт
            map.MapProvider = GMap.NET.MapProviders.OpenStreetMapProvider.Instance;
            // Устанавливаем минимальный и максимальный уровни масштабирования
            map.MinZoom = 2;
            map.MaxZoom = 17;
            // Устанавливаем начальный масштаб
            map.Zoom = 3;
            // Устанавливаем начальные координаты центра (примерно центральная точка России)
            map.Position = new PointLatLng(64.6863, 97.7453);
            // Позволяем использовать колесо мыши для масштабирования карты
            map.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            // Разрешаем перемещение карты с помощью мыши
            map.CanDragMap = true;
            // Устанавливаем кнопку мыши для перетаскивания карты (левая кнопка мыши)
            map.DragButton = MouseButton.Left;
            // Скрываем центральный крест на карте
            map.ShowCenter = false;
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

        // Метод для центрировании карты над основным городам
        public static void SetCenterZoomView(GMapControl map, double latitude, double longitude)
        {
            map.Position = new PointLatLng(latitude, longitude); // Устанавливаем координаты центра
            map.Zoom = 4; // Устанавливаем начальный масштаб
        }


        public static void AddSquare(GMapControl map, double latitude, double longitude, string cityName, Brush color)
        {
            // Создаем квадратную метку на карту Gmap.NET
            Rectangle square = new Rectangle()
            {
                Width = 10,
                Height = 10,
                Fill = color, // Цвет квадрата
                Opacity = 0.8
            };

            // При наведении мыши на квадрат показываем название города
            square.ToolTip = cityName;

            // Устанавливаем координаты квадрата
            GMapMarker marker = new GMapMarker(new PointLatLng(latitude, longitude));
            marker.Shape = square;

            // Добавляем квадрат на карту
            map.Markers.Add(marker);
        }

        public static void AddSquareMarkers(GMapControl map)
        {
            // Получаем данные о городах из базы данных
            List<(string cityName, double latitude, double longitude, int climate)> cityData = DataBaseSQLite.GetCityData();

            // Добавляем квадратные метки для каждого города
            foreach (var data in cityData)
            {
                // Определяем цвет квадратной метки в зависимости от значения в столбце "climate"
                Brush color;
                switch (data.climate)
                {
                    case 1:
                        color = Brushes.Blue;
                        break;
                    case 2:
                        color = Brushes.Yellow;
                        break;
                    case 3:
                        color = Brushes.Red;
                        break;
                    default:
                        color = Brushes.Black;
                        break;
                }

                // Добавляем квадратную метку на карту
                AddSquare(map, data.latitude, data.longitude, data.cityName, color);
            }
        }


        public static void ToggleClimateLayer(GMapControl map)
        {
            // Проверяем, включен ли уже климатический слой
            if (ClimateLayerActive(map))
            {
                RemoveClimateLayer(map); // Если да, то закрываем его
            }
            else
            {
                RemoveEcologicalLayer(map); // Закрываем предыдущий слой перед открытием климатического
                AddSquareMarkers(map); // Открываем климатический слой
            }
        }


        private static bool ClimateLayerActive(GMapControl map)
        {
            return map.Markers.Any(marker => marker.Shape is Rectangle);
        }

        public static void RemoveClimateLayer(GMapControl map)
        {
            // Удаляем все маркеры, которые являются квадратами
            var squareMarkers = map.Markers.Where(marker => marker.Shape is Rectangle).ToList();
            foreach (var marker in squareMarkers)
            {
                map.Markers.Remove(marker);
            }
        }


        public static void AddEcologicalMarkers(GMapControl map)
        {
            // Получаем данные о городах из базы данных
            List<(string cityName, double latitude, double longitude, int ecology)> cityData = DataBaseSQLite.GetEcologicalData();

            // Добавляем маркеры для каждого города
            foreach (var data in cityData)
            {
                // Определяем цвет маркера в зависимости от значения экологии
                Brush color;
                switch (data.ecology)
                {
                    case 1:
                        color = Brushes.Green;
                        break;
                    case 2:
                        color = Brushes.Yellow;
                        break;
                    case 3:
                        color = Brushes.Red;
                        break;
                    default:
                        color = Brushes.Black;
                        break;
                }

                // Добавляем маркер на карту
                AddSquare(map, data.latitude, data.longitude, data.cityName, color);
            }
        }

        public static void ToggleEcologicalLayer(GMapControl map)
        {
            // Проверяем, включен ли уже экологический слой
            if (EcologicalLayerActive(map))
            {
                RemoveEcologicalLayer(map); // Если да, то закрываем его
            }
            else
            {
                RemoveClimateLayer(map); // Закрываем предыдущий слой перед открытием экологического
                AddEcologicalMarkers(map); // Открываем экологический слой
            }
        }

        private static bool EcologicalLayerActive(GMapControl map)
        {
            return map.Markers.Any(marker => marker.Shape is Rectangle);
        }

        public static void RemoveEcologicalLayer(GMapControl map)
        {
            // Удаляем все маркеры, которые являются прямоугольниками
            var squareMarkers = map.Markers.Where(marker => marker.Shape is Rectangle).ToList();
            foreach (var marker in squareMarkers)
            {
                map.Markers.Remove(marker);
            }
        }
    }
}

