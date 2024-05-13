using GMap.NET.WindowsPresentation;
using GMap.NET;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VogeltrekWPF.Scripts;


namespace VogeltrekWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<int> selectedAnswersFull { get; set; }
        private GMapMarker primaryCityMarker = null; // Глобальная переменная для хранения метки основного города
        private GMapMarker ratingCityMarker = null; //Глобальная переменная для хранения метки города из списка рейтинга

        //Конструктор первого запуска без параметров
        public MainWindow()
        {
            InitializeComponent();
            GmapSheet.ConfigureMap(mapSurvey);
            DataBaseSQLite.LoadDataFromDB(listRatingCities, ComboBoxCityResidence);

            CommandBindings.Add(new CommandBinding(Scripts.MenuCommands.NewProject, Scripts.MenuCommands.NewProject_Executed));
            CommandBindings.Add(new CommandBinding(Scripts.MenuCommands.OpenProject, Scripts.MenuCommands.OpenProject_Executed));
            CommandBindings.Add(new CommandBinding(Scripts.MenuCommands.Exit, Scripts.MenuCommands.Exit_Executed));
        }


        //Тестовая кнопка перехода к опросу
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Создаем новое окно опроса
            QuestionnaireWindow questionnaireWindow = new QuestionnaireWindow();

            // Устанавливаем ссылку на главное окно в окне опроса
            questionnaireWindow.ParentWindow = this;

            // Отображаем окно опроса
            questionnaireWindow.Show();

            // Ожидаем закрытия окна опроса
            questionnaireWindow.Closed += (s, args) =>
            {
                // Выводим полученный список в консоль в одной строке
                Console.WriteLine("Список выбранных ответов в MainWindow: " + string.Join(", ", this.selectedAnswersFull));
                
                List<string> sortedCities = DataBaseSQLite.SortCitiesByRating(selectedAnswersFull);

                // Обновляем список городов в listRatingCities
                listRatingCities.ItemsSource = sortedCities;
                
                // Проверяем, выбран ли основной город
                if (ComboBoxCityResidence.SelectedItem != null)
                {
                    // Отображаем соединительные линии к основному городу
                    string selectedCity = ComboBoxCityResidence.SelectedItem as string;
                    (double primaryLatitude, double primaryLongitude) = DataBaseSQLite.GetCityCoordinates(selectedCity);
                    foreach (string city in sortedCities.Take(5))
                    {
                        (double cityLatitude, double cityLongitude) = DataBaseSQLite.GetCityCoordinates(city);
                        GmapSheet.AddRoute(mapSurvey, primaryLatitude, primaryLongitude, cityLatitude, cityLongitude, city);
                    }
                }
            };
        }


        // Добавляем метку на карту при выборе основного города из ComboBoxCityResidence
        private void ComboBoxCityResidence_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Получаем выбранный город из списка
            string selectedCity = (sender as ComboBox).SelectedItem as string;

            // Получаем координаты выбранного города из базы данных
            if (!string.IsNullOrEmpty(selectedCity))
            {
                (double latitude, double longitude) = DataBaseSQLite.GetCityCoordinates(selectedCity);

                // Очищаем карту от предыдущих меток и маршрутов
                GmapSheet.ClearMap(mapSurvey);

                // Добавляем новую метку основного города на карту Gmap.NET
                primaryCityMarker = GmapSheet.AddMarker(mapSurvey, latitude, longitude, isPrimaryCity: true);
                Console.WriteLine("Выбранный город: " + selectedCity);
            }
        }


        //Отображение метки при выборе города из списка рейтинга
        private void listRatingCities_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Очищаем список меток городов рейтинга, если он не пуст
            if (ratingCityMarker != null)
            {
                mapSurvey.Markers.Remove(ratingCityMarker);
            }

            // Получаем выбранный город из списка
            string selectedCity = listRatingCities.SelectedItem as string;

            // Получаем координаты выбранного города из базы данных
            if (!string.IsNullOrEmpty(selectedCity))
            {
                (double latitude, double longitude) = DataBaseSQLite.GetCityCoordinates(selectedCity);

                // Добавляем метку города рейтинга на карту Gmap.NET
                ratingCityMarker = GmapSheet.AddMarker(mapSurvey, latitude, longitude);
            }
        }
    }
}
