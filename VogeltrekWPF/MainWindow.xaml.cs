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
        //Конструктор первого запуска без параметров
        public MainWindow()
        {
            InitializeComponent();
            
            CommandBindings.Add(new CommandBinding(Scripts.MenuCommands.NewProject, Scripts.MenuCommands.NewProject_Executed));
            CommandBindings.Add(new CommandBinding(Scripts.MenuCommands.OpenProject, Scripts.MenuCommands.OpenProject_Executed));
            CommandBindings.Add(new CommandBinding(Scripts.MenuCommands.Exit, Scripts.MenuCommands.Exit_Executed));
        }


        //Конструктор с полученными ответами из окна Опроса
        public List<int> SelectedAnswers { get; private set; }
        public MainWindow(List<int> selectedAnswers)
        {
            InitializeComponent();

            // Инициализация переменной выбранных ответов
            SelectedAnswers = selectedAnswers;
            // Вывод списка ответов в консоль для проверки
            Console.WriteLine("Вопросы в окне MainWindow: " + string.Join(", ", SelectedAnswers));
        }


        private void mapSurvey_Loaded(object sender, RoutedEventArgs e)
        {
            GmapSheet.ConfigureMap(mapSurvey);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Открываем окно с результатами
            QuestionnaireWindow mainWindow = new QuestionnaireWindow();

            mainWindow.Show();
            // Закрываем текущее окно
            Close();
        }
    }
}
