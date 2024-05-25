using GMap.NET.MapProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace VogeltrekWPF.Scripts
{
    internal static class MenuCommands
    {
        // Определение команды с помощью RoutedUICommand
        public static readonly RoutedUICommand SavePicture = new RoutedUICommand("Сохранить результаты", "SavePicture", typeof(MenuCommands));
        public static readonly RoutedUICommand Exit = new RoutedUICommand("Выход", "Exit", typeof(MenuCommands));
        public static readonly RoutedUICommand ClimateLayer = new RoutedUICommand("Климатический слой", "ClimateLayer", typeof(MenuCommands));
        public static readonly RoutedUICommand EcologicalLayer = new RoutedUICommand("Экологический слой", "EcologicalLayer", typeof(MenuCommands));
        public static readonly RoutedUICommand DefaultZoom = new RoutedUICommand("По умолчанию", "DefaultZoom", typeof(MenuCommands));
        public static readonly RoutedUICommand CenterZoom = new RoutedUICommand("Центрировать", "CenterZoom", typeof(MenuCommands));
        public static readonly RoutedUICommand ChangeMapType = new RoutedUICommand("Сменить тип карты", "ChangeMapType", typeof(MenuCommands));

        // Команда для сохранения изображения карты
        public static void SavePicture_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is MainWindow window)
            {
                GmapSheet.SaveMapAsImage(window.mapSurvey);
            }
        }


        // Команда для выхода из приложения
        public static void Exit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // Обработка команды "Выход"
            if (sender is Window window)
            {
                window.Close();
            }
        }


        // Команда для переключения климатического слоя
        public static void ClimateLayer_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is MainWindow window)
            {
                // Вызываем метод для переключения климатического слоя
                GmapSheet.ToggleClimateLayer(window.mapSurvey);
            }
        }


        // Команда для отображения меток городов по экологическому слою
        public static void EcologicalLayer_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is MainWindow window)
            {
                // Вызываем метод для отображения меток городов по экологическому слою
                GmapSheet.ToggleEcologicalLayer(window.mapSurvey);
            }
        }

        // Команда для установки масштаба по умолчанию
        public static void DefaultZoom_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is MainWindow window)
            {
                GmapSheet.SetDefaultView(window.mapSurvey);
            }
        }

        // Команда для центрирования карты на выбранном городе
        public static void CenterZoom_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is MainWindow window)
            {
                // Получаем координаты выбранного города
                (double latitude, double longitude) = DataBaseSQLite.GetCityCoordinates(window.ComboBoxCityResidence.SelectedItem as string);

                // Передаем координаты в метод SetCenterZoomView
                GmapSheet.SetCenterZoomView(window.mapSurvey, latitude, longitude);
            }
        }


        // Команда для смены типа карты
        public static void ChangeMapType_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is MainWindow window)
            {
                string selectedMapType = e.Parameter as string;

                switch (selectedMapType)
                {
                    case "Топографическая":
                        window.mapSurvey.MapProvider = GMapProviders.OpenStreetMap;
                        break;
                    case "Ландшафтная":
                        window.mapSurvey.MapProvider = GMapProviders.OpenCycleLandscapeMap;
                        break;
                    case "WikiMapia":
                        window.mapSurvey.MapProvider = GMapProviders.WikiMapiaMap;
                        break;
                    default:
                        window.mapSurvey.MapProvider = GMapProviders.OpenStreetMap;
                        break;
                }
            }

        }
    }
}
