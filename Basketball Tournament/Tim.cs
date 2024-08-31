using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basketball_Tournament
{
    public class Tim
    {
        public string Team { get; set; } = "";
        public string ISOCode { get; set; } = "";
        public int FIBARanking { get; set; }
        public int PointsScored { get; set; }
        public int PointsConceded {  get; set; }

        public Group Group { get; set; } = new Group();
        public int PointsInGroup {  get; set; }
        
        public int? OverallRank { get; set; } //    After groups rank
        public int? PointDifferential { get; set; } 

        public Tim(string team, string iSOCode, int fIBARanking)
        {
            Team = team;
            ISOCode = iSOCode;
            FIBARanking = fIBARanking;
        }

        public Tim() { }

        

    }







}
