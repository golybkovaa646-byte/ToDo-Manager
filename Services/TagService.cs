using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using ToDo_Manager.Date;
using ToDo_Manager.Models;
using ToDo_Manager.Services.Interface;

namespace ToDo_Manager.Services
{
    public class TagService : ITagService
    {
        private readonly IDbContextFactory<ToDoContext> _factory;

        public TagService(IDbContextFactory<ToDoContext> factory)
        {
            _factory = factory;
        }

        public async Task<List<Tag>> GetAllAsync()
        {
            using var db = _factory.CreateDbContext();
            return await db.Tags.AsNoTracking().ToListAsync();
        }

        public async Task AddAsync(Tag tag)
        {
            using var db = _factory.CreateDbContext();
            db.Tags.Add(tag);
            await db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Tag tag)
        {
            using var db = _factory.CreateDbContext();
            db.Tags.Remove(tag);
            await db.SaveChangesAsync();
        }
    }
}
