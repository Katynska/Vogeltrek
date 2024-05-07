using System;
using System.Collections.Generic;
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
        public List<int> SelectedAnswers { get; set; }
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
                Console.WriteLine("Список выбранных ответов в MainWindow: " + string.Join(", ", this.SelectedAnswers));
            };
        }


        private void ComboBoxCityResidence_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Вывод выбранного города в консоль
            string selectedCity = (sender as ComboBox).SelectedItem as string;
            if (!string.IsNullOrEmpty(selectedCity))
            {
                Console.WriteLine("Выбранный город: " + selectedCity);
            }
        }
    }
}
