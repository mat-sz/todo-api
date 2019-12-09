using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TodoAPI.Entities
{
    public class TodoList : ITimestamped
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ProjectId { get; set; }

        [JsonIgnore]
        public Project Project { get; set; }
        public List<TodoItem> TodoItems { get; set; } = new List<TodoItem>();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}