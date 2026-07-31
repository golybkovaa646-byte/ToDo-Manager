using System;
using System.Collections.Generic;
using System.Text;
using ToDo_Manager.Date;
using ToDo_Manager.Models;
using Microsoft.EntityFrameworkCore;
using ToDo_Manager.Services.Interface;

namespace ToDo_Manager.Services
{
    public class TaskService : ITaskService
    {
        private readonly IDbContextFactory<ToDoContext> _factory;

        public TaskService(IDbContextFactory<ToDoContext> factory)
        {
            _factory = factory;
        }

        public async Task<List<TaskItem>> GetAllAsync()
        {
            using var db = _factory.CreateDbContext();
            return await db.Tasks.AsNoTracking().ToListAsync();
        }

        public async Task AddAsync(TaskItem item)
        {
            using var db = _factory.CreateDbContext();
            db.Tasks.Add(item);
            await db.SaveChangesAsync();
        }

        public async Task DeleteAsync(TaskItem item)
        {
            using var db = _factory.CreateDbContext();
            db.Tasks.Remove(item);
            await db.SaveChangesAsync();
        }

        public async Task UpdateAsync(TaskItem item)
        {
            using var db = _factory.CreateDbContext();
            db.Tasks.Update(item);
            await db.SaveChangesAsync();
        }

    }

}
