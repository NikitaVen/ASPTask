using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace web_app3.Models
{
    public class EnablingUsers
    {
        public List<int> _IDs { set; get; }
        public List<bool> _Checkboxes { set; get; }
        public EnablingUsers()
        {
            _IDs = new List<int>();
            _Checkboxes = new List<bool>();
        }

        public EnablingUsers(List<User> users)
        {
            _IDs = new List<int>(users.Count);
            _Checkboxes = new List<bool>(users.Count);
            for (int i = 0; i < users.Count; ++i)
            {
                _IDs.Add(users[i].ID);
                _Checkboxes.Add(false);
            }
        }
    }
}
