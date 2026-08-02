using System.Windows;
using ToDo_Manager.Models;
using ToDo_Manager.ViewModels;

namespace ToDo_Manager.View
{
    public partial class EditTaskWindow : Window
    {


        public EditTaskWindow()
        {
            InitializeComponent();
            Loaded += EditTaskWindow_Loaded;
        }


        private void EditTaskWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is EditTaskViewModel vm)
            {
                vm.RequestClose += () =>
                {
                    DialogResult = true;
                    Close();
                };
            }
        }
    }
}
