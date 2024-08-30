using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basketball_Tournament
{
    public class Tim
    {
        public string Team { get; set; }
        public string ISOCode { get; set; }
        public int FIBARanking {  get; set; }

        public Tim(string team, string iSOCode, int fIBARanking)
        {
            Team = team;
            ISOCode = iSOCode;
            FIBARanking = fIBARanking;
        }
    }



}
