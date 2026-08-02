using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<List<Tag>> GetAllTagsAsync()
        {
            using var db = _factory.CreateDbContext();
            return await db.Tags.AsNoTracking().ToListAsync();
        }

        public async Task<List<TaskItem>> GetAllAsync()
        {
            using var db = _factory.CreateDbContext();
            return await db.Tasks
                .Include(t => t.TaskTags)
                    .ThenInclude(tt => tt.Tag)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<TaskItem?> GetByIdWithTagsAsync(int id)
        {
            using var db = _factory.CreateDbContext();
            return await db.Tasks
                .Include(t => t.TaskTags)
                    .ThenInclude(tt => tt.Tag)
                .FirstOrDefaultAsync(t => t.Id == id);
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

            var existing = await db.Tasks
                .Include(t => t.TaskTags)
                .FirstOrDefaultAsync(t => t.Id == item.Id);

            if (existing == null)
                return;

            existing.Title = item.Title;
            existing.Description = item.Description;
            existing.Priority = item.Priority;

            // Сохраняем связи, как раньше (удаляем и добавляем из item.TaskTags)
            var existingLinks = db.Set<TaskTag>().Where(tt => tt.TaskItemId == existing.Id).ToList();
            if (existingLinks.Any())
                db.Set<TaskTag>().RemoveRange(existingLinks);

            if (item.TaskTags != null)
            {
                foreach (var tt in item.TaskTags)
                {
                    db.Set<TaskTag>().Add(new TaskTag
                    {
                        TaskItemId = existing.Id,
                        TagId = tt.TagId
                    });
                }
            }

            await db.SaveChangesAsync();
        }

        // Новый метод: жёстко удалить все связи и добавить одну (если tagId != null)
        public async Task SetTagForTaskAsync(int taskItemId, int? tagId)
        {
            using var db = _factory.CreateDbContext();

            // Убедимся, что задача существует
            var task = await db.Tasks.FirstOrDefaultAsync(t => t.Id == taskItemId);
            if (task == null)
                return;

            // Удаляем все существующие связи
            var existingLinks = db.Set<TaskTag>().Where(tt => tt.TaskItemId == taskItemId).ToList();
            if (existingLinks.Any())
                db.Set<TaskTag>().RemoveRange(existingLinks);

            // Если передан tagId — добавляем новую связь
            if (tagId.HasValue)
            {
                db.Set<TaskTag>().Add(new TaskTag
                {
                    TaskItemId = taskItemId,
                    TagId = tagId.Value
                });
            }

            await db.SaveChangesAsync();
        }
    }
}
