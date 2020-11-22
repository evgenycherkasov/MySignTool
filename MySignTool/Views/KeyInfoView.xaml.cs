using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MySignTool.Views
{
    /// <summary>
    /// Логика взаимодействия для KeyInfoView.xaml
    /// </summary>
    public partial class KeyInfoView : Window
    {
        public KeyInfoView(BindableBase VM)
        {
            InitializeComponent();
            DataContext = VM;
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
