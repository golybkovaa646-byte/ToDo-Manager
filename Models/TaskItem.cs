using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;
using ToDo_Manager.Date;

namespace ToDo_Manager.Models
{
    public partial class TaskItem : ObservableObject
    {
        public int Id { get; set; }

        [ObservableProperty]
        private string title = string.Empty;

        [ObservableProperty]
        private bool isCompleted;

        [ObservableProperty]
        private Priority priority = Priority.Medium;

        public DateTime CreatedAt { get; set; } = DateTime.Now;


        partial void OnIsCompletedChanged(bool oldValue, bool newValue)
        {
            using var db = new ToDoContext();
            db.Tasks.Update(this);
            db.SaveChanges();
        }

    }


}
