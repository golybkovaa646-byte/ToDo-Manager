using System;
using System.Collections.Generic;
using System.Text;

namespace ToDo_Manager.Models
{
    public class Tag
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        
        public string ColorHex { get; set; } = "#808080";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<TaskTag> TaskTags { get; set; } = new List<TaskTag>();
    }
}
