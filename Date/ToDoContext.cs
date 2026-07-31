using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using ToDo_Manager.Models;
using System.IO;

namespace ToDo_Manager.Date
{
    public class ToDoContext : DbContext
    {
        public DbSet<TaskItem> Tasks => Set<TaskItem>();

        public ToDoContext()
        {
            
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "todo.db");
            options.UseSqlite($"Data Source={path}");
        }

    }
}
