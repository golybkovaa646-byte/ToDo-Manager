using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using ToDo_Manager.Models;
using ToDo_Manager.Services.Interface;

public partial class EditTaskViewModel : ObservableObject
{
    private readonly ITaskService _taskService;
    private readonly IMessageService _messageService;

    public ObservableCollection<Priority> Priorities { get; } =
    new ObservableCollection<Priority>(Enum.GetValues(typeof(Priority)).Cast<Priority>());

    public event Action? RequestClose;
    public TaskItem OriginalTask { get; }

    [ObservableProperty]
    private string title;

    [ObservableProperty]
    private string description;

    [ObservableProperty]
    private Priority priority;

    public EditTaskViewModel(TaskItem task, ITaskService taskService, IMessageService messageService)
    {
        OriginalTask = task;
        _taskService = taskService;
        _messageService = messageService;

        Title = task.Title;
        Priority = task.Priority;
        Description = task.Description;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        OriginalTask.Title = Title;
        OriginalTask.Description = Description;
        OriginalTask.Priority = Priority;

        await _taskService.UpdateAsync(OriginalTask);
        _messageService.ShowInfo("The task has been successfully updated!");
        RequestClose?.Invoke();
    }

    [RelayCommand]
    private void Cancel() => RequestClose?.Invoke();
}
