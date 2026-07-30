using Microsoft.Extensions.DependencyInjection;
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
using ToDo_Manager.Models;
using ToDo_Manager.Services.Interface;
using ToDo_Manager.ViewModels;

namespace ToDo_Manager.View
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;

        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            var task = (TaskItem)((Button)sender).DataContext;

            var vm = new EditTaskViewModel(task,
                                           App.AppHost.Services.GetRequiredService<ITaskService>(),
                                           App.AppHost.Services.GetRequiredService<IMessageService>());

            var window = new EditTaskWindow(vm);
            window.ShowDialog();
        }



    }
}
