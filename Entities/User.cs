using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TodoAPI.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        
        [JsonIgnore]
        public string Password { get; set; }
        public List<UserProject> UserProjects { get; set; } = new List<UserProject>();
    }
}