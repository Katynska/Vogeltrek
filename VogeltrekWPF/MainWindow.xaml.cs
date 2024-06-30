using GMap.NET.WindowsPresentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
        private bool isFilterEnabled = false; // Переменная для отслеживания состояния фильтра

        //Конструктор первого запуска без параметров
        public MainWindow()
        {
            InitializeComponent();
            GmapSheet.ConfigureMap(mapSurvey);
            DataBaseSQLite.LoadDataFromDB(listRatingCities, ComboBoxCityResidence);

            // Добавляем обработчик события Checked для CheckBox
            this.CheckBoxPopulation.Checked += CheckBox_Checked;
            SliderPopulation.ValueChanged += SliderPopulation_ValueChanged;

            CommandBindings.Add(new CommandBinding(Scripts.MenuCommands.SavePicture, Scripts.MenuCommands.SavePicture_Executed));
            CommandBindings.Add(new CommandBinding(Scripts.MenuCommands.Exit, Scripts.MenuCommands.Exit_Executed));
            CommandBindings.Add(new CommandBinding(Scripts.MenuCommands.ClimateLayer, Scripts.MenuCommands.ClimateLayer_Executed));
            CommandBindings.Add(new CommandBinding(Scripts.MenuCommands.EcologicalLayer, Scripts.MenuCommands.EcologicalLayer_Executed));
            CommandBindings.Add(new CommandBinding(Scripts.MenuCommands.DefaultZoom, Scripts.MenuCommands.DefaultZoom_Executed));
            CommandBindings.Add(new CommandBinding(Scripts.MenuCommands.CenterZoom, Scripts.MenuCommands.CenterZoom_Executed));
            CommandBindings.Add(new CommandBinding(Scripts.MenuCommands.ChangeMapType, MenuCommands.ChangeMapType_Executed));
            CommandBindings.Add(new CommandBinding(Scripts.MenuCommands.InformationHelp, MenuCommands.InformationHelp_Executed));
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
                //Console.WriteLine("Список выбранных ответов в MainWindow: " + string.Join(", ", this.selectedAnswersFull));

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
                primaryCityMarker = GmapSheet.AddMarker(mapSurvey, latitude, longitude, selectedCity, isPrimaryCity: true);
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
                ratingCityMarker = GmapSheet.AddMarker(mapSurvey, latitude, longitude, selectedCity);
            }
        }


        //Кнопка увеличение масштаба
        private void ZoomIN_Click(object sender, MouseButtonEventArgs e)
        {
            mapSurvey.Zoom += 1; // Увеличиваем масштаб карты на единицу при каждом нажатии
        }
        //Кнопка уменьшение масштаба
        private void ZoomOut_Click(object sender, MouseButtonEventArgs e)
        {
            mapSurvey.Zoom -= 1; // Увеличиваем масштаб карты на единицу при каждом нажатии
        }


        //Кнопка фильтра отображение всех гордов
        private void Filter_Click(object sender, MouseButtonEventArgs e)
        {
            // Инвертируем состояние фильтра
            isFilterEnabled = !isFilterEnabled;

            if (isFilterEnabled)
            {
                // Включаем отображение всех городов из списка
                // Перебираем все элементы в списке рейтинга городов и отображаем их на карте
                foreach (string city in listRatingCities.Items)
                {
                    (double latitude, double longitude) = DataBaseSQLite.GetCityCoordinates(city);
                    GmapSheet.AddCircle(mapSurvey, latitude, longitude, city, Brushes.Teal);
                }
            }
            else
            {
                // Удаляем все метки городов с карты
                GmapSheet.ClearMap(mapSurvey);
            }
        }

        //Обработка вкл чекбокса
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            // Вызываем метод SliderPopulation_ValueChanged для первоначальной отрисовки меток
            SliderPopulation_ValueChanged(SliderPopulation, new RoutedPropertyChangedEventArgs<double>(SliderPopulation.Value, SliderPopulation.Value));
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            // Очищаем карту от всех меток при деактивации чекбокса
            GmapSheet.ClearMap(mapSurvey);
            // Сбрасываем заголовок чекбокса
            CheckBoxPopulation.Content = "Вкл";
        }


        private void SliderPopulation_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (CheckBoxPopulation.IsChecked == true)
            {
                // Получаем значение промежутка из SliderPopulation
                int populationThreshold = (int)e.NewValue;

                // Загружаем данные и отображаем круги городов на карте
                GmapSheet.ClearMap(mapSurvey); // Очищаем карту от предыдущих меток
                int count = 0; // Переменная для подсчета количества меток
                foreach (string cityName in listRatingCities.Items.Cast<string>().ToList())
                {
                    (double latitude, double longitude) = DataBaseSQLite.GetCityCoordinates(cityName);
                    int population = DataBaseSQLite.GetCityPopulation(cityName);

                    // Проверяем, попадает ли численность населения в заданный промежуток
                    if (population <= populationThreshold)
                    {
                        // Добавляем круг города на карту
                        GmapSheet.AddCircle(mapSurvey, latitude, longitude, cityName, Brushes.OrangeRed);
                        count++; // Увеличиваем счетчик меток
                    }
                }
                // Выводим количество меток в заголовок чекбокса
                CheckBoxPopulation.Content = $"Городов ({count})";
            }
        }
    }
}
