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
    /// Логика взаимодействия для AnswerControl.xaml
    /// </summary>
    public partial class AnswerControl : UserControl
    {
        Answer answer;

        public AnswerControl(Answer answer)
        {
            InitializeComponent();
            this.answer = answer;
            this.DataContext = answer;
        }
    }
}
