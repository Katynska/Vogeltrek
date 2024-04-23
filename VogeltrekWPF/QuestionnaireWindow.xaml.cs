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
    public partial class QuestionnaireWindow : Window
    {
        public QuestionnaireWindow()
        {
            InitializeComponent();
        }

        // Обработчик события для кнопок "Ответить"
        private void NextQuestion_Click(object sender, RoutedEventArgs e)
        {
            // Ваш код для перехода к следующему вопросу здесь
        }
    }
}
