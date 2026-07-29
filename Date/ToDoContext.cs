using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using ToDo_Manager.Models;


namespace ToDo_Manager.Date
{
    public class ToDoContext : DbContext
    {
        public DbSet<TaskItem> Tasks => Set<TaskItem>();

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite("Data Source=todo.db");
        }
    }
}
