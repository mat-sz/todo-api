using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TodoAPI.Entities
{
    public class Label
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public int ProjectId { get; set; }

        [JsonIgnore]
        public Project Project { get; set; }
        
        [JsonIgnore]
        public List<TodoItemLabel> TodoItemLabels { get; set; } = new List<TodoItemLabel>();
    }
}