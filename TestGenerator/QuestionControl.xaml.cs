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

namespace TestGenerator
{
    /// <summary>
    /// Логика взаимодействия для QuestionControl.xaml
    /// </summary>
    public partial class QuestionControl : UserControl
    {
        Question question;

        public QuestionControl(Question question)
        {
            InitializeComponent();
            this.question = question;
            this.DataContext = question;
            foreach (var answer in question.Answers)
            {
                AnswerControl ac = new AnswerControl(answer);
                AnswersSP.Children.Add(ac);
            }
        }
    }
}
