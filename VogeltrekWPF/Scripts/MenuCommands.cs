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
            public static readonly RoutedUICommand NewProject = new RoutedUICommand("New Project", "NewProject", typeof(MenuCommands));
            public static readonly RoutedUICommand OpenProject = new RoutedUICommand("Open Project", "OpenProject", typeof(MenuCommands));
            public static readonly RoutedUICommand Exit = new RoutedUICommand("Выход", "Exit", typeof(MenuCommands));
        

        public static void NewProject_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // Обработка команды "New Project"
        }

        public static void OpenProject_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // Обработка команды "Open Project"
        }

        public static void Exit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // Обработка команды "Выход"
            if (sender is Window window)
            {
                window.Close();
            }
        }
    }
}
