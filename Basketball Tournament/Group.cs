using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basketball_Tournament
{
    public class Group(string groupName, List<Tim> teams)
    {
        public string GroupName { get; set; } = groupName;
        public List<Tim> Teams { get; set; } = teams;
    }
}
