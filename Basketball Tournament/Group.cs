using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basketball_Tournament
{
    public class Group
    {
        public string GroupName { get; set; }
        public List<Tim> Teams { get; set; }
        public List<Match> Matches { get; set; } = [];

        public Group(string groupName, List<Tim> teams)
        {
            GroupName = groupName;
            Teams = teams;
        }

        public Group() { }
    }
}
