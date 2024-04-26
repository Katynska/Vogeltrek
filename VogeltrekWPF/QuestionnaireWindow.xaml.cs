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
using System.Windows.Shapes;

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
        private List<Answer> selectedAnswers = new List<Answer>(); // Массив для хранения выбранных ответов

        public QuestionnaireWindow()
        {
            InitializeComponent();
            InitializeQuestions();
            DataContext = this;
        }

        // Инициализация списка вопросов
        private void InitializeQuestions()
        {
            questions.Add(new Question
            {
                Text = "Как вы реагируете на понедельник?",
                Answers = new List<Answer>
                {
                    new Answer { Text = "Жизнь – это один сплошной приключенческий роман! Понедельник? Это просто новая страница для написания наших захватывающих историй!" },
                    new Answer { Text = "После второй чашки кофе я воспринимаю его немного лучше" },
                    new Answer { Text = "Понедельник? Пожалуй, лучше переждать его в кровати" },
                    new Answer { Text = "Я принимаю его как вызов и готов к покорению новых вершин!" },
                    new Answer { Text = "Я просто игнорирую его существование и пытаюсь продлить выходные"},
                    new Answer { Text = "Понедельник! Это как готовиться к сражению с трёхголовым драконом, только без меча и щита, а с чашкой кофе и с печенькой!" }
                }
            });
            questions.Add(new Question
            {
                Text = "Сколько снегу нужно, чтобы зима была идеальной?",
                Answers = new List<Answer>
                {
                    new Answer { Text = "Лучше без снега вообще! Давайте пляжи и пальмы!" },
                    new Answer { Text = "Немного для красоты, и хватит! Пара хлопьев для настроения — и нормально"},
                    new Answer { Text = "Если это не снежный буран, то чем больше, тем лучше! Я готов к любым снежным приключениям!"}
                }
            });
            // Добавьте другие вопросы....

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
            var radioButtonList = mainPanel.Children
                .OfType<StackPanel>()
                .FirstOrDefault(stackPanel => stackPanel.Tag?.ToString() == currentQuestionIndex.ToString())
                ?.Children
                .OfType<RadioButton>();

            if (radioButtonList != null)
            {
                foreach (var radioButton in radioButtonList)
                {
                    if (radioButton.IsChecked == true)
                    {
                        var selectedAnswer = new Answer
                        {
                            Text = radioButton.Content.ToString(),
                            IsSelected = true
                        };
                        selectedAnswers.Add(selectedAnswer);
                        return;
                    }
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
            {
                // Если дошли до конца, можно что-то сделать, например, закрыть окно или показать сообщение об окончании опроса
                Close(); // Пример: закрываем окно
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
            var currentQuestionPanel = mainPanel.Children.OfType<StackPanel>().FirstOrDefault(panel => panel.IsVisible);
            if (currentQuestionPanel == null)
            {
                return false;
            }

            var itemsControl = currentQuestionPanel.Children.OfType<ItemsControl>().FirstOrDefault();
            if (itemsControl == null)
            {
                return false;
            }

            var radioButtons = FindVisualChildren<RadioButton>(itemsControl);
            return radioButtons.Any(radioButton => radioButton.IsChecked.HasValue && radioButton.IsChecked.Value);
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
                currentQuestionIndex--; // Переходим к предыдущему вопросу
                UpdateCurrentQuestionVisibility(); // Обновляем интерфейс для предыдущего вопроса
            }
            else
            {
                MessageBox.Show("Это первый вопрос.");
            }
        }
    }
}

