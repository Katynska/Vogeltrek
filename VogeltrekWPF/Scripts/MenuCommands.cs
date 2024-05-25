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
        public static void SavePicture_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is MainWindow window)
            {
                GmapSheet.SaveMapAsImage(window.mapSurvey);
            }
        }
        public static void Exit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // Обработка команды "Выход"
            if (sender is Window window)
            {
                window.Close();
            }
        }

        public static void ClimateLayer_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // Обработка команды "Open Project"
        }


        public static void EcologicalLayer_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // Обработка команды "Open Project"
        }

        public static void DefaultZoom_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is MainWindow window)
            {
                GmapSheet.SetDefaultView(window.mapSurvey);
            }
        }

        public static void CenterZoom_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // Обработка команды "Open Project"
        }


    }
}
