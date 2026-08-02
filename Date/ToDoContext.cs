using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using ToDo_Manager.Models;

namespace ToDo_Manager.Date
{
    public class ToDoContext : DbContext
    {
        public DbSet<TaskItem> Tasks => Set<TaskItem>();
        public DbSet<Tag> Tags => Set<Tag>();
        public DbSet<TaskTag> TaskTags => Set<TaskTag>();

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "ToDoManager"
            );
            Directory.CreateDirectory(appDataPath);
            var dbPath = Path.Combine(appDataPath, "todo.db");
            options.UseSqlite($"Data Source={dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaskTag>()
                .HasKey(tt => new { tt.TaskItemId, tt.TagId });

            modelBuilder.Entity<TaskTag>()
                .HasOne(tt => tt.TaskItem)
                .WithMany(t => t.TaskTags)
                .HasForeignKey(tt => tt.TaskItemId);

            modelBuilder.Entity<TaskTag>()
                .HasOne(tt => tt.Tag)
                .WithMany(t => t.TaskTags)
                .HasForeignKey(tt => tt.TagId);
        }

        /// <summary>
        /// Универсальный метод применения миграций.
        /// Обрабатывает все сценарии, в случае ошибки пересоздаёт БД с бэкапом.
        /// </summary>
        public void ApplyMigrations()
        {
            var dbPath = GetDatabasePath();
            var backupPath = dbPath + ".backup";

            try
            {
                // 1. Если БД не существует – создаём новую
                if (!File.Exists(dbPath))
                {
                    Console.WriteLine("Database not found. Creating new...");
                    Database.EnsureCreated();
                    Console.WriteLine("New database created.");
                    return;
                }

                // 2. Проверяем наличие таблицы миграций
                bool hasMigrationTable = CheckIfMigrationTableExists();

                if (hasMigrationTable)
                {
                    // 3. БД с историей – применяем стандартные миграции
                    Console.WriteLine("Database with migration history. Applying migrations...");
                    try
                    {
                        Database.Migrate();
                        Console.WriteLine("Migrations applied.");
                        return;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Migration error: {ex.Message}");
                        // При ошибке – пересоздаём
                        RecreateDatabaseWithBackup(backupPath);
                        return;
                    }
                }
                else
                {
                    // 4. Старая БД без истории – ручная миграция
                    Console.WriteLine("Old database without migration history. Manual migration...");
                    try
                    {
                        MigrateOldDatabase();
                        Console.WriteLine("Manual migration successful.");
                        return;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Manual migration error: {ex.Message}");
                        RecreateDatabaseWithBackup(backupPath);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                RecreateDatabaseWithBackup(backupPath);
            }
        }

        // ========== ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ ==========

        private string GetDatabasePath()
        {
            var appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "ToDoManager"
            );
            return Path.Combine(appDataPath, "todo.db");
        }

        private bool CheckIfMigrationTableExists()
        {
            try
            {
                using var connection = Database.GetDbConnection();
                connection.Open();
                using var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='__EFMigrationsHistory'";
                var result = cmd.ExecuteScalar();
                connection.Close();
                return result != null;
            }
            catch
            {
                // В случае ошибки считаем, что таблицы нет – попытаемся восстановить
                return false;
            }
        }

        private void RecreateDatabaseWithBackup(string backupPath)
        {
            try
            {
                var dbPath = GetDatabasePath();

                // Создаём бэкап старой БД (если существует)
                if (File.Exists(dbPath))
                {
                    File.Copy(dbPath, backupPath, true);
                    Console.WriteLine($"Database backed up to: {backupPath}");
                }

                // Удаляем старую БД
                if (File.Exists(dbPath))
                    File.Delete(dbPath);

                // Создаём новую
                Database.EnsureCreated();
                Console.WriteLine("New database created (previous data backed up).");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CRITICAL: Failed to recreate database: {ex.Message}");
                throw new Exception("Unable to initialize database. Please contact support.", ex);
            }
        }

        private void MigrateOldDatabase()
        {
            // 1. Создаём таблицу миграций (если отсутствует)
            Database.ExecuteSqlRaw(@"
                CREATE TABLE IF NOT EXISTS ""__EFMigrationsHistory"" (
                    ""MigrationId"" TEXT NOT NULL CONSTRAINT ""PK___EFMigrationsHistory"" PRIMARY KEY,
                    ""ProductVersion"" TEXT NOT NULL
                );
            ");
            Console.WriteLine("Migration history table created.");

            // 2. Проверяем и обновляем таблицу Tasks
            if (CheckIfTableExists("Tasks"))
            {
                Console.WriteLine("Updating Tasks table...");
                AddColumnIfNotExists("Tasks", "TagId", "INTEGER");
                AddColumnIfNotExists("Tasks", "CreatedAt", "DATETIME DEFAULT CURRENT_TIMESTAMP");
                // Добавьте сюда все новые колонки, которые появились в новой версии
            }

            // 3. Создаём Tags (если нет)
            if (!CheckIfTableExists("Tags"))
            {
                Console.WriteLine("Creating Tags table...");
                Database.ExecuteSqlRaw(@"
                    CREATE TABLE Tags (
                        Id INTEGER NOT NULL CONSTRAINT PK_Tags PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL
                    );
                ");
            }

            // 4. Создаём TaskTags (если нет)
            if (!CheckIfTableExists("TaskTags"))
            {
                Console.WriteLine("Creating TaskTags table...");
                Database.ExecuteSqlRaw(@"
                    CREATE TABLE TaskTags (
                        TaskItemId INTEGER NOT NULL,
                        TagId INTEGER NOT NULL,
                        CONSTRAINT PK_TaskTags PRIMARY KEY (TaskItemId, TagId),
                        CONSTRAINT FK_TaskTags_Tasks_TaskItemId FOREIGN KEY (TaskItemId) REFERENCES Tasks (Id) ON DELETE CASCADE,
                        CONSTRAINT FK_TaskTags_Tags_TagId FOREIGN KEY (TagId) REFERENCES Tags (Id) ON DELETE CASCADE
                    );
                ");
            }

            // 5. Записываем фиктивную миграцию (чтобы EF считал, что всё применено)
            var migrationId = "20250101000000_InitialCreate"; // Замените на имя вашей первой миграции
            var productVersion = GetEFCoreVersion();
            Database.ExecuteSqlRaw(@"
                INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
                VALUES ({0}, {1});
            ", migrationId, productVersion);

            Console.WriteLine($"Migration recorded: {migrationId} (v{productVersion})");
        }

        private bool CheckIfTableExists(string tableName)
        {
            try
            {
                using var connection = Database.GetDbConnection();
                connection.Open();
                using var cmd = connection.CreateCommand();
                cmd.CommandText = $"SELECT name FROM sqlite_master WHERE type='table' AND name='{tableName}'";
                var result = cmd.ExecuteScalar();
                connection.Close();
                return result != null;
            }
            catch
            {
                return false;
            }
        }

        private void AddColumnIfNotExists(string tableName, string columnName, string columnType)
        {
            try
            {
                // Проверяем наличие колонки через PRAGMA table_info
                using var connection = Database.GetDbConnection();
                connection.Open();
                using var cmd = connection.CreateCommand();
                cmd.CommandText = $"PRAGMA table_info({tableName})";
                using var reader = cmd.ExecuteReader();
                bool exists = false;
                while (reader.Read())
                {
                    if (reader["name"].ToString() == columnName)
                    {
                        exists = true;
                        break;
                    }
                }
                reader.Close();
                connection.Close();

                if (!exists)
                {
                    Database.ExecuteSqlRaw($"ALTER TABLE {tableName} ADD COLUMN {columnName} {columnType};");
                    Console.WriteLine($"Column '{columnName}' added.");
                }
                else
                {
                    Console.WriteLine($"Column '{columnName}' already exists.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding column '{columnName}': {ex.Message}");
                // Не прерываем миграцию
            }
        }

        private string GetEFCoreVersion()
        {
            var version = typeof(DbContext).Assembly.GetName().Version;
            return $"{version.Major}.{version.Minor}.{version.Build}";
        }
    }
}