using System;
using System.Collections.Generic;
using System.Text;
using ToDo_Manager.Models;

namespace ToDo_Manager.Services.Interface
{
    public interface ITaskService
    {
        Task<List<TaskItem>> GetAllAsync();
        Task AddAsync(TaskItem item);
        Task<TaskItem?> GetByIdWithTagsAsync(int id);
        Task DeleteAsync(TaskItem item);
        Task UpdateAsync(TaskItem item);

        Task<List<Tag>> GetAllTagsAsync();

        Task SetTagForTaskAsync(int taskItemId, int? tagId);
    }
}
