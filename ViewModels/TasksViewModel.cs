using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using ToDo_Manager.Models;
using ToDo_Manager.Services.Interface;

namespace ToDo_Manager.ViewModels
{
    public partial class TasksViewModel : ObservableObject
    {
        private readonly ITaskService _taskService;
        private readonly IMessageService _messageService;
        private readonly IDialogService _dialogService;

        

        public ObservableCollection<Priority> Priorities { get; } =
            new ObservableCollection<Priority>(Enum.GetValues(typeof(Priority)).Cast<Priority>());

        [ObservableProperty]
        private Priority newTaskPriority = Priority.Medium;

        [ObservableProperty]
        private string newTaskTitle = string.Empty;

        [ObservableProperty]
        private string newTaskDescription = string.Empty;

        [ObservableProperty]
        private Priority? selectedFilterPriority;

        [ObservableProperty]
        private bool showCompleted = false;

        public ObservableCollection<TaskItem> Tasks { get; } = new();
        public ObservableCollection<TaskItem> FilteredTasks { get; } = new();


        public int TotalTasksCount => Tasks?.Count ?? 0;
        public int FilteredTasksCount => FilteredTasks?.Count ?? 0;

        public TasksViewModel(ITaskService taskService, IMessageService messageService, IDialogService dialogService)
        {
            _taskService = taskService;
            _messageService = messageService;
            _dialogService = dialogService;

            LoadTasks();
        }

        partial void OnSelectedFilterPriorityChanged(Priority? oldValue, Priority? newValue)
        {
            RefreshFilteredTasks();
        }

        partial void OnShowCompletedChanged(bool oldValue, bool newValue)
        {
            RefreshFilteredTasks();
        }

        partial void OnNewTaskTitleChanged(string oldValue, string newValue)
        {
            if (newValue.Length > 25)
            {
                NewTaskTitle = newValue[..25];
                _messageService.ShowInfo("Maximum 25 characters. Your text will be shortened.");
            }
        }

        private void RefreshFilteredTasks()
        {
            FilteredTasks.Clear();

            foreach (var t in Tasks)
            {
                if ((SelectedFilterPriority == null || t.Priority == SelectedFilterPriority)
                    && (ShowCompleted ? t.IsCompleted : true))
                {
                    FilteredTasks.Add(t);
                }
            }
            OnPropertyChanged(nameof(TotalTasksCount));
            OnPropertyChanged(nameof(FilteredTasksCount));
        }

        private async void LoadTasks()
        {
            try
            {
                var items = await _taskService.GetAllAsync();

                Tasks.Clear();
                foreach (var item in items)
                    Tasks.Insert(0, item);
                OnPropertyChanged(nameof(TotalTasksCount));
                RefreshFilteredTasks();
            }
            catch (Exception ex)
            {
                _messageService.ShowError($"Error loading tasks: {ex.Message}");
            }
        }

        [RelayCommand]
        private async void AddTask()
        {
            if (string.IsNullOrWhiteSpace(NewTaskTitle))
            {
                _messageService.ShowInfo("Task title cannot be empty.");
                return;
            }
            if (string.IsNullOrWhiteSpace(NewTaskDescription))
            {
                _messageService.ShowInfo("Task description cannot be empty.");
                return;
            }

            try
            {
                var task = new TaskItem
                {
                    Title = NewTaskTitle,
                    Description = NewTaskDescription,
                    Priority = NewTaskPriority
                };

                await _taskService.AddAsync(task);
                Tasks.Insert(0, task);
                OnPropertyChanged(nameof(TotalTasksCount));
                RefreshFilteredTasks();

                NewTaskDescription = string.Empty;
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
                if (!_messageService.ShowConfrime($"Are you sure you want to delete the task '{task.Title}'?"))
                    return;

                await _taskService.DeleteAsync(task);
                Tasks.Remove(task);
                OnPropertyChanged(nameof(TotalTasksCount));
                RefreshFilteredTasks();
            }
            catch (Exception ex)
            {
                _messageService.ShowError($"Error deleting task: {ex.Message}");
            }
        }

        [RelayCommand]
        private void OpenEditWindow(TaskItem task)
        {
            _dialogService.EditTask(task);
        }

        [RelayCommand]
        private async void EditTask(TaskItem task)
        {
            try
            {
                await _taskService.UpdateAsync(task);
                RefreshFilteredTasks();
            }
            catch (Exception ex)
            {
                _messageService.ShowError($"Error editing task: {ex.Message}");
            }
        }

        [RelayCommand]
        private void ClearFilters()
        {
            SelectedFilterPriority = null;
            ShowCompleted = false;
            RefreshFilteredTasks();
        }
    }
}
