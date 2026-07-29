using System.Windows;
using ToDo_Manager.Models;
using ToDo_Manager.ViewModels;

namespace ToDo_Manager.View
{
    public partial class EditTaskWindow : Window
    {


        public EditTaskWindow(EditTaskViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;

            vm.RequestClose += Close;
        }
    }
}
