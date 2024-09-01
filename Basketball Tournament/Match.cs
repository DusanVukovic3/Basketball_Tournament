namespace Basketball_Tournament
{
    public class Match
    {
        public Tim Team1 { get; set; }
        public Tim Team2 { get; set; }
        public string Result { get; set; } = "";    // 100 : 83

        public Match(Tim team1, Tim team2)
        {
            Team1 = team1;
            Team2 = team2;
        }

        public void SetResult(int scoreA, int scoreB)
        {
            Result = $"{scoreA} : {scoreB}";
        }

        public (int scoreA, int scoreB) GetScores()
        {
            var scores = Result.Split(':');
            int scoreA = int.Parse(scores[0].Trim());
            int scoreB = int.Parse(scores[1].Trim());
            return (scoreA, scoreB);
        }

        public Tim GetWinner()
        {
            var (scoreA, scoreB) = GetScores();
            return scoreA > scoreB ? Team1 : Team2;
        }

        public Tim GetLoser()
        {
            var (scoreA, scoreB) = GetScores();
            return scoreA > scoreB ? Team2 : Team1;
        }


    }
}
