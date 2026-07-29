using System;
using System.Collections.Generic;
using System.Text;
using ToDo_Manager.Date;
using ToDo_Manager.Models;
using Microsoft.EntityFrameworkCore;

namespace ToDo_Manager.Services
{
    public class TaskService : ITaskService
    {
        private readonly ToDoContext _db;

        public TaskService(ToDoContext db)
        {
            _db = db;
        }

        public async Task<List<TaskItem>> GetAllAsync()
        {
            return await _db.Tasks.AsTracking().ToListAsync();
        }

        public async Task AddAsync(TaskItem item)
        {
            _db.Tasks.Add(item);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(TaskItem item)
        {
            _db.Tasks.Remove(item);
            await _db.SaveChangesAsync();
        }
    }

}
