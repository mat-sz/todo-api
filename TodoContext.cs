using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TodoAPI.Entities;

namespace TodoAPI
{
    public class TodoContext : DbContext
    {
        public DbSet<Project> Projects { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserProject> UserProjects { get; set; }
        public DbSet<TodoList> TodoLists { get; set; }
        public DbSet<TodoItem> TodoItems { get; set; }
        public DbSet<Label> Labels { get; set; }
        public DbSet<TodoItemLabel> TodoItemLabels { get; set; }
        
        public TodoContext(DbContextOptions<TodoContext> options) : base(options)
        {
        }

        public override int SaveChanges()
        {
            var newEntities = this.ChangeTracker.Entries()
                .Where(x => x.State == EntityState.Added &&
                    x.Entity != null &&
                    x.Entity as ITimestamped != null)
                .Select(x => x.Entity as ITimestamped);

            var modifiedEntities = this.ChangeTracker.Entries() 
                .Where(x => x.State == EntityState.Modified &&
                    x.Entity != null &&
                    x.Entity as ITimestamped != null)
                .Select(x => x.Entity as ITimestamped);

            foreach (var newEntity in newEntities)
            {
                newEntity.CreatedAt = DateTime.UtcNow;
                newEntity.UpdatedAt = DateTime.UtcNow;
            }

            foreach (var modifiedEntity in modifiedEntities)
            {
                modifiedEntity.UpdatedAt = DateTime.UtcNow;
            }

            return base.SaveChanges();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserProject>()
                .HasKey(up => new { up.UserId, up.ProjectId });  

            modelBuilder.Entity<UserProject>()
                .HasOne(up => up.User)
                .WithMany(u => u.UserProjects)
                .HasForeignKey(up => up.UserId);  

            modelBuilder.Entity<UserProject>()
                .HasOne(up => up.Project)
                .WithMany(p => p.UserProjects)
                .HasForeignKey(up => up.ProjectId);

            modelBuilder.Entity<TodoItemLabel>()
                .HasKey(til => new { til.TodoItemId, til.LabelId });  

            modelBuilder.Entity<TodoItemLabel>()
                .HasOne(til => til.TodoItem)
                .WithMany(ti => ti.TodoItemLabels)
                .HasForeignKey(til => til.TodoItemId);  

            modelBuilder.Entity<TodoItemLabel>()
                .HasOne(til => til.Label)
                .WithMany(l => l.TodoItemLabels)
                .HasForeignKey(til => til.LabelId);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();
        }
    }
}