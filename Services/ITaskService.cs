using System;
using System.Collections.Generic;
using System.Text;
using ToDo_Manager.Models;

namespace ToDo_Manager.Services
{
    public interface ITaskService
    {
        Task<List<TaskItem>> GetAllAsync();
        Task AddAsync(TaskItem item);
        Task DeleteAsync(TaskItem item);
        Task UpdateAsync(TaskItem item);

    }
}
