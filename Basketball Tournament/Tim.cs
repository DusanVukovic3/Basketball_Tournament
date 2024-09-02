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

        private static readonly Random random = new();


        public Tim SimulateGame(Tim team1, Tim team2)
        {
            double rankDifference = Math.Abs(team1.FIBARanking - team2.FIBARanking);    // Based on difference in rank, simulate score
            double scoreDifference = rankDifference * random.NextDouble();  

            int score1 = random.Next(60, 120) + (int)(scoreDifference / 2);  // Team will get at least 60 points and at most 120
            int score2 = random.Next(60, 120) - (int)(scoreDifference / 2);

            while (score1 == score2)
            {
                HandleOvertime(ref score1, ref score2, team1, team2);  
            }

            Console.WriteLine($"{team1.Team} vs {team2.Team} ({score1}:{score2})");

            
            Match match = new(team1, team2);
            match.SetResult(score1, score2);

            MatchesList.Add(match);  // Ensure MatchesList is initialized correctly

            UpdateTeamPoints(score1, score2, team1, team2);
            UpdateScoringStats(score1, score2, team1, team2);

            return score1 > score2 ? team1 : team2; 
        }

        private static void HandleOvertime(ref int score1, ref int score2, Tim team1, Tim team2)
        {
            int overtimePointsTeam1 = random.Next(5, 20);   // Teams can get 5-20 points in 1 overtime
            int overtimePointsTeam2 = random.Next(5, 20);

            score1 += overtimePointsTeam1;
            score2 += overtimePointsTeam2;

            Console.WriteLine($"\nOvertime: {team1.Team} vs {team2.Team} ({overtimePointsTeam1}:{overtimePointsTeam2}) ");
        }

        private static void UpdateScoringStats(int score1, int score2, Tim team1, Tim team2)
        {
            team1.PointsScored += score1;
            team1.PointsConceded += score2;
            team2.PointsScored += score2;
            team2.PointsConceded += score1;
        }


        private static void UpdateTeamPoints(int score1, int score2, Tim team1, Tim team2)
        {
            if (score1 > score2)
            {
                team1.PointsInGroup += 2;  // Update points (rename if needed for knockout phase)
                team2.PointsInGroup += 1;
            }
            else
            {
                team1.PointsInGroup += 1;
                team2.PointsInGroup += 2;
            }
        }


        public List<Tim> SimulateRound(List<Match> matches)    //  Get all the winners of the match
        {
            var winners = new List<Tim>();

            foreach (var match in matches)
            {
                var winner = SimulateGame(match.Team1, match.Team2);
                winners.Add(winner);
            } 
            return winners;
        }


    }







}
