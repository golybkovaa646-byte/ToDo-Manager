using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ToDo_Manager.Models;
using ToDo_Manager.Services.Interface;

public partial class EditTaskViewModel : ObservableObject
{
    private readonly ITaskService _taskService;
    private readonly IMessageService _messageService;

    public ObservableCollection<Priority> Priorities { get; } =
        new ObservableCollection<Priority>(Enum.GetValues(typeof(Priority)).Cast<Priority>());

    public ObservableCollection<Tag> AvailableTags { get; } = new();

    public event Action? RequestClose;
    public TaskItem OriginalTask { get; }

    [ObservableProperty]
    private string title;

    [ObservableProperty]
    private string description;

    [ObservableProperty]
    private Priority priority;

    [ObservableProperty]
    private Tag? selectedTag;

    public EditTaskViewModel(TaskItem task, ITaskService taskService, IMessageService messageService)
    {
        OriginalTask = task;
        _taskService = taskService;
        _messageService = messageService;

        Title = task.Title;
        Priority = task.Priority;
        Description = task.Description;

        LoadTags();

        var currentTag = task.TaskTags.FirstOrDefault()?.Tag;
        if (currentTag != null)
        {
            SelectedTag = AvailableTags.FirstOrDefault(t => t.Id == currentTag.Id);
        }
    }

    private async void LoadTags()
    {
        var tags = await _taskService.GetAllTagsAsync();
        AvailableTags.Clear();
        foreach (var tag in tags)
            AvailableTags.Add(tag);
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        
        OriginalTask.Title = Title;
        OriginalTask.Description = Description;
        OriginalTask.Priority = Priority;

 
        int? tagId = SelectedTag?.Id;

        await _taskService.SetTagForTaskAsync(OriginalTask.Id, tagId);

        
        var updated = await _taskService.GetByIdWithTagsAsync(OriginalTask.Id);
        if (updated != null)
        {
            
            OriginalTask.Title = updated.Title;
            OriginalTask.Description = updated.Description;
            OriginalTask.Priority = updated.Priority;

            OriginalTask.TaskTags.Clear();
            foreach (var tt in updated.TaskTags)
                OriginalTask.TaskTags.Add(tt);
        }

        _messageService.ShowInfo("The task has been successfully updated!");
        RequestClose?.Invoke();
    }

    [RelayCommand]
    private void Cancel() => RequestClose?.Invoke();
}
