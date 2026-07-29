using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows;
using ToDo_Manager.Date;
using ToDo_Manager.Models;
using ToDo_Manager.Services;

namespace ToDo_Manager.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly ToDoContext _db;
        private readonly IMessageService _messageService;

        public ObservableCollection<Priority> Priorities { get; } =
                new ObservableCollection<Priority>(Enum.GetValues(typeof(Priority)).Cast<Priority>());

        [ObservableProperty]
        private Priority newTaskPriority = Priority.Medium;

        [ObservableProperty]
        private string newTaskTitle = string.Empty;

        public ObservableCollection<TaskItem> Tasks { get; set; } = new();

        public MainViewModel(ToDoContext db, IMessageService messageService)
        {
            _db = db;
            _messageService = messageService;
            LoadTasks();
        }

        [RelayCommand]
        private async void AddTask()
        {
            if (string.IsNullOrWhiteSpace(NewTaskTitle))
                return;
            try
            {
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
            catch (Exception ex)
            {
                _messageService.ShowError($"Error adding task: {ex.Message}");
            }
        }


        [RelayCommand]
        private async void DeleteTask(TaskItem task)
        {
            try
            {
                _db.Tasks.Remove(task);
                await _db.SaveChangesAsync();
                Tasks.Remove(task);
            }
            catch (Exception ex)
            {
                _messageService.ShowError($"Error deleting task: {ex.Message}");
            }
        }

        private async void LoadTasks()
        {
            try
            {
                var items = await _db.Tasks.AsTracking().ToListAsync();
                Tasks.Clear();
                foreach (var item in items)
                    Tasks.Add(item);
            }
            catch (Exception ex)
            {
                _messageService.ShowError($"Error loading tasks: {ex.Message}");
            }
        }

    
        

        partial void OnNewTaskTitleChanged(string oldValue, string newValue)
        {
            
        }
    }
}
