using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace web_app3.Models
{
    public enum status
    {
        Active = 1,
        Block = 0
    }
    public class User
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime RegistrationTime { get; set; }
        public DateTime LastLoginTime { get; set; }
        public status Status { get; set; }
    }
}
