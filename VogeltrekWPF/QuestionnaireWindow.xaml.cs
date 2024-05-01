using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
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
using System.Windows.Shapes;
using System.Windows.Resources;


namespace VogeltrekWPF
{
    /// <summary>
    /// Логика взаимодействия для QuestionnaireWindow.xaml
    /// </summary>

    // Класс для представления ответа
    public class Answer
    {
        public string Text { get; set; } // Текст ответа
        public bool IsSelected { get; set; } // // Флаг, указывающий, выбран ли этот ответ
        public int Rating { get; set; } // Новое свойство для рейтинга
    }


    // Класс для представления вопроса
    public class Question
    {
        public string Text { get; set; } // Текст вопроса
        public List<Answer> Answers { get; set; } // Список возможных ответов на вопрос
    }


    // Определение класса окна опроса
    public partial class QuestionnaireWindow : Window
    {
        private List<Question> questions = new List<Question>(); // Список вопросов
        private int currentQuestionIndex = 0;  // Индекс текущего вопроса
        private List<int> selectedAnswers = new List<int>(); // Массив для хранения выбранных ответов

        public QuestionnaireWindow()
        {
            InitializeComponent();
            InitializeQuestions();
            DataContext = this;
        }


        // Инициализация списка вопросов
        private void InitializeQuestions()
        {
            // Получение пути к JSON файлу
            string jsonFilePath = "/Resources/Questions.json";

            // Получение потока ресурса
            Uri UriQuestions = new Uri(jsonFilePath, UriKind.Relative);
            StreamResourceInfo resourceInfo = Application.GetResourceStream(UriQuestions);

            if (resourceInfo == null)
            {
                MessageBox.Show("Не удалось найти файл questions.json в папке Resources.");
                return;
            }

            // Чтение JSON из потока
            using (StreamReader reader = new StreamReader(resourceInfo.Stream))
            {
                string json = reader.ReadToEnd();
                // Десериализация JSON в список вопросов
                questions = JsonConvert.DeserializeObject<List<Question>>(json);
            }

            // Создаем StackPanel'ы для каждого вопроса и добавляем их в mainPanel
            InitializeQuestionsUI();
        }


        // Инициализация интерфейса для каждого вопроса
        private void InitializeQuestionsUI()
        {
            for (int i = 0; i < questions.Count; i++)
            {
                StackPanel questionPanel = new StackPanel();
                questionPanel.Name = "questionPanel" + i;
                questionPanel.Tag = i;
                questionPanel.Visibility = Visibility.Collapsed;

                TextBlock textBlock = new TextBlock();
                textBlock.Text = questions[i].Text;
                textBlock.FontSize = 18;
                textBlock.Margin = new Thickness(0, 20, 0, 0);

                ItemsControl itemsControl = new ItemsControl();
                itemsControl.ItemsSource = questions[i].Answers;
                itemsControl.ItemTemplate = (DataTemplate)Resources["RadioButtonTemplate"];

                questionPanel.Children.Add(textBlock);
                questionPanel.Children.Add(itemsControl);

                mainPanel.Children.Add(questionPanel);
            }

            // Показываем первый вопрос
            if (mainPanel.Children.Count > 0)
            {
                ((StackPanel)mainPanel.Children[0]).Visibility = Visibility.Visible;
            }
        }


        // Метод для сохранения выбранного ответа
        private void SaveSelectedAnswer()
        {
            // Находим текущий вопрос
            var currentQuestion = questions.ElementAtOrDefault(currentQuestionIndex);
            if (currentQuestion != null)
            {
                // Ищем выбранный ответ
                var selectedAnswer = currentQuestion.Answers.FirstOrDefault(a => a.IsSelected);
                if (selectedAnswer != null)
                {
                    selectedAnswers.Add(selectedAnswer.Rating);
                }
            }
        }


        // Метод для перехода к следующему вопросу и обновления интерфейса
        private void MoveToNextQuestionAndUpdate()
        {
            SaveSelectedAnswer(); // Сохраняем выбранный ответ

            currentQuestionIndex++; // Переходим к следующему вопросу

            // Проверяем, не дошли ли мы до конца опроса
            if (currentQuestionIndex < questions.Count)
            {
                UpdateCurrentQuestionVisibility(); // Обновляем интерфейс для нового вопроса
            }
            else
            {   //Вывод списка ответов в консоль для проверки
                Console.WriteLine("Selected answers: " + string.Join(", ", selectedAnswers));

                // Скрываем все вопросы и ответы вместе с кнопками "Ответить" и "Назад"
                mainPanel.Visibility = Visibility.Collapsed;
                AcceptAnswerButton.Visibility = Visibility.Collapsed;
                RollbackAnswerButton.Visibility = Visibility.Collapsed;

                // Изменяем текст заголовка на "Опрос завершен!"
                TextBlockTitle.Text = "Опрос завершен!";
                // Отображение кнопки "Перейти к результатам"
                ShowResultsButton.Visibility = Visibility.Visible;
            }
        }


        // Метод для обновления видимости текущего вопроса
        private void UpdateCurrentQuestionVisibility()
        {
            foreach (UIElement element in mainPanel.Children)
            {
                if (element is StackPanel panel && panel.Tag?.ToString() == currentQuestionIndex.ToString())
                {
                    panel.Visibility = Visibility.Visible;
                }
                else if (element is StackPanel)
                {
                    (element as StackPanel).Visibility = Visibility.Collapsed;
                }
            }
        }


        // Метод для проверки, выбран ли ответ на текущий вопрос
        private bool IsAnswerSelectedForCurrentQuestion()
        {
            var currentQuestion = questions.ElementAtOrDefault(currentQuestionIndex);
            if (currentQuestion != null)
            {
                return currentQuestion.Answers.Any(a => a.IsSelected);
            }
            return false;
        }


        // Рекурсивный метод для поиска всех визуальных дочерних элементов заданного типа T в визуальном дереве элемента depObj
        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T t)
                    {
                        yield return t;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }


        // Обработчик события для кнопки "Ответить"
        private void AcceptAnswer_Click(object sender, RoutedEventArgs e)
        {
            if (!IsAnswerSelectedForCurrentQuestion())
            {
                MessageBox.Show("Пожалуйста, выберите ответ.");
                return;
            }

            MoveToNextQuestionAndUpdate();
        }


        // Обработчик события для кнопки "Назад"
        private void RollbackAnswer_Click(object sender, RoutedEventArgs e)
        {
            if (currentQuestionIndex > 0)
            {
                selectedAnswers.RemoveAt(selectedAnswers.Count - 1); // Удаляем последний выбранный ответ из списка
                currentQuestionIndex--; // Переходим к предыдущему вопросу
                UpdateCurrentQuestionVisibility(); // Обновляем интерфейс для предыдущего вопроса
            }
            else
            {
                MessageBox.Show("Это первый вопрос.");
            }
        }


        // Обработчик события для кнопки "Перейти к результатам"
        private void ShowResults_Click(object sender, RoutedEventArgs e)
        {
            // Открываем окно с результатами
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();

            // Закрываем текущее окно
            Close();
        }
    }
}

