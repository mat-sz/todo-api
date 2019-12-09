using System;

namespace TodoAPI.Entities
{
    public class TodoItem : ITimestamped
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Done { get; set; }
        public int TodoListId { get; set; }
        public TodoList TodoList { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}