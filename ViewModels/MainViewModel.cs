using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Configuration;
using System.Windows.Controls;
using ToDo_Manager.View.Pages;

namespace ToDo_Manager.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private Page? currentPage;

        public MainViewModel()
        {
            CurrentPage = new TasksPage(); 
        }

        [RelayCommand]
        private void NavigateTasks()
        {
            CurrentPage = new TasksPage();
        }
        [RelayCommand]
        private void NavigateTags()
        {
            CurrentPage = new TagsPage();
        }
       
    }
}
