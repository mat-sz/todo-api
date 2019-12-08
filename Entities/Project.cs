using System.Collections.Generic;

namespace TodoAPI.Entities
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<UserProject> UserProjects { get; set; } = new List<UserProject>();
        public List<TodoList> TodoLists { get; set; } = new List<TodoList>();
    }
}