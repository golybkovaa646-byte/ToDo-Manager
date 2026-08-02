using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;
using ToDo_Manager.Date;
using System.ComponentModel.DataAnnotations.Schema;


namespace ToDo_Manager.Models
{
    public partial class TaskItem : ObservableObject
    {
        public int Id { get; set; }

        [ObservableProperty]
        private string title = string.Empty;

        [ObservableProperty]
        private string description = string.Empty;


        [ObservableProperty]
        private bool isCompleted;

        [NotMapped]
        [ObservableProperty]
        private bool isEditing;

        [ObservableProperty]
        private Priority priority = Priority.Medium;

        public DateTime CreatedAt { get; set; } = DateTime.Now;


        partial void OnIsCompletedChanged(bool oldValue, bool newValue)
        {
           /* using var db = new ToDoContext();
            db.Tasks.Update(this);
            db.SaveChanges();*/
        }

        public ICollection<TaskTag> TaskTags { get; set; } = new List<TaskTag>();


    }


}
