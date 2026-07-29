using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ToDo_Manager.Models;
using ToDo_Manager.Services;

public partial class EditTaskViewModel : ObservableObject
{
    private readonly ITaskService _taskService;
    private readonly IMessageService _messageService;
    public event Action? RequestClose;
    public TaskItem OriginalTask { get; }

    [ObservableProperty]
    private string title;

    [ObservableProperty]
    private Priority priority;

    public EditTaskViewModel(TaskItem task, ITaskService taskService, IMessageService messageService)
    {
        OriginalTask = task;
        _taskService = taskService;
        _messageService = messageService;

        // копируем данные
        Title = task.Title;
        Priority = task.Priority;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        OriginalTask.Title = Title;
        OriginalTask.Priority = Priority;

        await _taskService.UpdateAsync(OriginalTask);

        RequestClose?.Invoke();
    }


    [RelayCommand]
    private void Cancel()
    {
        RequestClose?.Invoke();
    }

}
