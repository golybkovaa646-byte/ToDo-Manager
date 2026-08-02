using System;
using System.Collections.Generic;
using System.Text;

namespace ToDo_Manager.Models
{
    public class TaskTag
    {
        public int TaskItemId { get; set; }
        public TaskItem TaskItem { get; set; }

        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }
}
