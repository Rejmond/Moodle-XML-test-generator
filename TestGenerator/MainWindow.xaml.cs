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
using System.IO;
using System.Xml.Linq;
using Microsoft.Win32;

namespace TestGenerator
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        InputControl inputControl;
        PreviewControl previewControl;
        ResultControl resultControl;
        Generator generator;
        AppStatus status;

        public MainWindow()
        {
            InitializeComponent();
            generator = new Generator();
            BackButton.IsEnabled = false;
            inputControl = new InputControl();
            ContentGrid.Children.Clear();
            ContentGrid.Children.Add(inputControl);
            status = AppStatus.Input;
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            switch (status)
            {
                case AppStatus.Input:
                    generator.InputText = inputControl.InputTB.Text;
                    if (generator.Parse()) { 
                        status = AppStatus.Preview;
                        BackButton.IsEnabled = true;
                        previewControl = new PreviewControl(generator.questions);
                        foreach (var question in generator.questions)
                        {
                            previewControl.QuestionsSP.Children.Add(new QuestionControl(question));
                        }
                        ContentGrid.Children.Clear();
                        ContentGrid.Children.Add(previewControl);
                    }
                    break;
                case AppStatus.Preview:
                    status = AppStatus.Result;
                    generator.GetXML();
                    resultControl = new ResultControl(generator.Xml);
                    ContentGrid.Children.Clear();
                    ContentGrid.Children.Add(resultControl);
                    break;
                case AppStatus.Result:
                    SaveFileDialog dialog = new SaveFileDialog();
                    dialog.Filter = "Moodle XML (*.xml)|*.xml";
                    dialog.DefaultExt = "xml";
                    if (dialog.ShowDialog() == true)
                    {
                        generator.Xml.Save(dialog.FileName);
                    }
                    break;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            switch (status)
            {
                case AppStatus.Preview:
                    status = AppStatus.Input;
                    BackButton.IsEnabled = false;
                    ContentGrid.Children.Clear();
                    ContentGrid.Children.Add(inputControl);
                    break;
                case AppStatus.Result:
                    status = AppStatus.Preview;
                    ContentGrid.Children.Clear();
                    ContentGrid.Children.Add(previewControl);
                    break;
            }
        }
    }

    enum AppStatus
    {
        Input,
        Preview,
        Result
    }
}
