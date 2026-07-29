using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using ToDo_Manager.Date;
using ToDo_Manager.Models;
using Microsoft.EntityFrameworkCore;

namespace ToDo_Manager.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly ToDoContext _db = new();

        public ObservableCollection<Priority> Priorities { get; } =
                new ObservableCollection<Priority>(Enum.GetValues(typeof(Priority)).Cast<Priority>());

        [ObservableProperty]
        private Priority newTaskPriority = Priority.Medium;

        [ObservableProperty]
        private string newTaskTitle = string.Empty;

        public ObservableCollection<TaskItem> Tasks { get; set; } = new();

        public MainViewModel()
        {
            LoadTasks();
        }

        [RelayCommand]
        private async void AddTask()
        {
            if (string.IsNullOrWhiteSpace(NewTaskTitle))
                return;

            var task = new TaskItem
            {
                Title = NewTaskTitle,
                Priority = NewTaskPriority
            };

            _db.Tasks.Add(task);
            await _db.SaveChangesAsync();

            Tasks.Add(task);
            NewTaskTitle = string.Empty;
            NewTaskPriority = Priority.Medium;
        }


        [RelayCommand]
        private async void DeleteTask(TaskItem task)
        {
            _db.Tasks.Remove(task);
            await _db.SaveChangesAsync();
            Tasks.Remove(task);
        }

        private async void LoadTasks()
        {
            var items = await _db.Tasks.AsTracking().ToListAsync();
            Tasks.Clear();
            foreach (var item in items)
                Tasks.Add(item);
        }

    
        

        partial void OnNewTaskTitleChanged(string oldValue, string newValue)
        {
            
        }
    }
}
