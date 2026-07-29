using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using ToDo_Manager.Models;
using ToDo_Manager.Services;

namespace ToDo_Manager.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly ITaskService _taskService;
        private readonly IMessageService _messageService;

        public ObservableCollection<Priority> Priorities { get; } =
            new ObservableCollection<Priority>(Enum.GetValues(typeof(Priority)).Cast<Priority>());

        [ObservableProperty]
        private Priority newTaskPriority = Priority.Medium;

        [ObservableProperty]
        private string newTaskTitle = string.Empty;

        public ObservableCollection<TaskItem> Tasks { get; } = new();

        public MainViewModel(ITaskService taskService, IMessageService messageService)
        {
            _taskService = taskService;
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

                await _taskService.AddAsync(task);
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
                await _taskService.DeleteAsync(task);
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
                var items = await _taskService.GetAllAsync();

                Tasks.Clear();
                foreach (var item in items)
                    Tasks.Add(item);
            }
            catch (Exception ex)
            {
                _messageService.ShowError($"Error loading tasks: {ex.Message}");
            }
        }

        [RelayCommand]
        private async void EditTask(TaskItem task)
        {
            try
            {
                await _taskService.UpdateAsync(task);
            }
            catch (Exception ex)
            {
                _messageService.ShowError($"Error editing task: {ex.Message}");
            }
        }

        [RelayCommand]
        private void StartEdit(TaskItem task)
        {
            task.IsEditing = true;
        }

        [RelayCommand]
        private async void FinishEdit(TaskItem task)
        {
            try
            {
                task.IsEditing = false;
                await _taskService.UpdateAsync(task);
            }
            catch (Exception ex)
            {
                _messageService.ShowError($"Error editing task: {ex.Message}");
            }
        }

        [RelayCommand]
        private void CancelEdit(TaskItem task)
        {
            task.IsEditing = false;
        }


    }
}
