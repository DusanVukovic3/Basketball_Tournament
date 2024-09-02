namespace Basketball_Tournament
{
    public class Match(Tim team1, Tim team2)
    {
        public Tim Team1 { get; set; } = team1;
        public Tim Team2 { get; set; } = team2;
        public string Result { get; set; } = "";    // 100 : 83

        public void SetResult(int scoreA, int scoreB)
        {
            Result = $"{scoreA} : {scoreB}";
        }

        public (int scoreA, int scoreB) GetScores()
        {
            if (string.IsNullOrWhiteSpace(Result))
            {
                throw new FormatException("Result is null or empty.");
            }

            var scores = Result.Split([':'], 2);  // Limit split to 2 parts

            if (scores.Length != 2)
            {
                throw new FormatException($"Invalid Result format: '{Result}'. Expected format is 'scoreA : scoreB'.");
            }

            string scoreAString = scores[0].Trim();
            string scoreBString = scores[1].Trim();

            if (!int.TryParse(scoreAString, out int scoreA) || !int.TryParse(scoreBString, out int scoreB))
            {
                throw new FormatException($"Invalid scores in Result: '{Result}'. Both parts must be valid integers.");
            }

            return (scoreA, scoreB);
        }


        public Tim GetWinner()
        {
            var (scoreA, scoreB) = GetScores();
            return scoreA > scoreB ? Team1 : Team2;
        }


    }
}
