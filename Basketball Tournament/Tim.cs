using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace Basketball_Tournament
{
    public class Tim
    {
        public string Team { get; set; } = "";
        public string ISOCode { get; set; } = "";
        public int FIBARanking { get; set; }
        public int PointsScored { get; set; }
        public int PointsConceded {  get; set; }
        public List<Match> MatchesList { get; set; } = [];
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

        

        public Tim SimulateGame(Tim team1, Tim team2)
        {
            var random = new Random();
            double rankDifference = (double)Math.Abs(team1.FIBARanking - team2.FIBARanking);    // Based on difference in rank, simulate score
            double scoreDifference = rankDifference * random.NextDouble();

            int score1 = random.Next(60, 120) + (int)(scoreDifference / 2);  // Team will get at least 60 points and at most 120
            int score2 = random.Next(60, 120) - (int)(scoreDifference / 2);

            Console.WriteLine($"{team1.Team} vs {team2.Team} ({score1}:{score2})");

            Match match = new(team1, team2);    //  Just after simulating match do we have a list with some elements   
            match.SetResult(score1, score2);
            MatchesList.Add(match);

            if (score1 > score2)
            {
                team1.PointsInGroup += 2;    // Group points, in knockout it will still count although won't matter
                team2.PointsInGroup += 1;
            }
            else
            {
                team1.PointsInGroup += 1;
                team2.PointsInGroup += 2;
            }

            team1.PointsScored += score1;
            team2.PointsConceded += score2;
            team1.PointsScored += score2;
            team1.PointsConceded += score1;

            // Return the winner
            return score1 > score2 ? team1 : team2;
        }

        public List<Tim> SimulateRound(List<(Tim, Tim)> matches)    //  Get all the winners of the match
        {
            var winners = new List<Tim>();

            foreach (var match in matches)
            {
                var winner = SimulateGame(match.Item1, match.Item2);
                winners.Add(winner);
            }
            
            return winners;
        }


    }







}
