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
    /// Логика взаимодействия для TagsPage.xaml
    /// </summary>
    public partial class TagsPage : Page
    {
        public TagsPage()
        {
            InitializeComponent();
            DataContext = new TagsViewModel(
                App.GetService<ITagService>(),
                App.GetService<IMessageService>());
        }
    }
}
