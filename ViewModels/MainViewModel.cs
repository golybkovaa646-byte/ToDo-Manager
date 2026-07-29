using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using ToDo_Manager.Date;
using ToDo_Manager.Models;
using Microsoft.EntityFrameworkCore;


namespace ToDo_Manager.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly ToDoContext _db = new();

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

            var task = new TaskItem { Title = NewTaskTitle };
            _db.Tasks.Add(task);
            await _db.SaveChangesAsync();

            Tasks.Add(task);
            NewTaskTitle = string.Empty;
        }

        [RelayCommand]
        private async void ToggleComplete(TaskItem task)
        {
            task.IsCompleted = !task.IsCompleted;
            _db.Tasks.Update(task);
            await _db.SaveChangesAsync();
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
            await _db.Database.EnsureCreatedAsync();
            var items = await _db.Tasks.ToListAsync();
            Tasks.Clear();
            foreach (var item in items)
                Tasks.Add(item);
        }
    }
}
