using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TodoAPI.Entities
{
    public class User : ITimestamped
    {
        public int Id { get; set; }
        public string Username { get; set; }
        
        [JsonIgnore]
        public string Password { get; set; }
        public List<UserProject> UserProjects { get; set; } = new List<UserProject>();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}