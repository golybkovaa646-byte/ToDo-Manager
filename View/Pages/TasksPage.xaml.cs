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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ToDo_Manager.Services.Interface;
using ToDo_Manager.ViewModels;

namespace ToDo_Manager.View.Pages
{
    /// <summary>
    /// Логика взаимодействия для TasksPage.xaml
    /// </summary>
    public partial class TasksPage : Page
    {
        public TasksPage()
        {
            InitializeComponent();
            DataContext = new TasksViewModel(
        App.GetService<ITaskService>(),
        App.GetService<IMessageService>(),
        App.GetService<IDialogService>());
        }
    }
}
