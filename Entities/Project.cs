using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TodoAPI.Entities
{
    public class Project : ITimestamped
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        [JsonIgnore]
        public List<UserProject> UserProjects { get; set; } = new List<UserProject>();
        public List<TodoList> TodoLists { get; set; } = new List<TodoList>();
        public List<Label> Labels { get; set; } = new List<Label>();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}