namespace Basketball_Tournament
{
    public class Match(Tim team1, Tim team2)
    {
        public Tim Team1 { get; set; } = team1;
        public Tim Team2 { get; set; } = team2;
        public string Result { get; set; } = "";    // 100 : 83

        private static readonly Random random = new();

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

        public Tim GetLoser()
        {
            var (scoreA, scoreB) = GetScores();
            return scoreA > scoreB ? Team2 : Team1;
        }


        /*public static List<Match> GenerateQuarterfinals(List<Tim> topTeams, int maxRetries = 100) //   Return 4 quarterfinal matches
        {
            List<Match>? quarterfinals = null;

            for (int attempt = 0; attempt < maxRetries; attempt++)
            {

                Shuffle(hatA, random);
                Shuffle(hatB, random);
                Shuffle(hatC, random);
                Shuffle(hatD, random);

                quarterfinals = [];

                var availableHatD = new List<Tim>(hatD);  // Copy of Hat 
                foreach (var teamA in hatA)
                {
                    var potentialOpponents = availableHatD.Where(teamD => teamD.Group != teamA.Group).ToList();

                    if (potentialOpponents.Count != 0)
                    {
                        var opponent = potentialOpponents[random.Next(potentialOpponents.Count)];
                        quarterfinals.Add(new Match(teamA, opponent));
                        availableHatD.Remove(opponent); // Remove selected opponent from available Hat D
                    }
                    else
                    {
                        break;  //  If we can't get valid pairs, retry
                    }
                }

                var availableHatC = new List<Tim>(hatC);
                foreach (var teamB in hatB)
                {
                    var potentialOpponents = availableHatC.Where(teamC => teamC.Group != teamB.Group).ToList();

                    if (potentialOpponents.Count != 0)
                    {
                        var opponent = potentialOpponents[random.Next(potentialOpponents.Count)];
                        quarterfinals.Add(new Match(teamB, opponent));
                        availableHatC.Remove(opponent); // Remove selected opponent from available Hat C
                    }
                    else
                    {
                        break;
                    }
                }

                if (quarterfinals.Count == 4)
                {
                    return quarterfinals;
                }
            }

            Console.WriteLine("Failed to generate valid quarterfinal pairs after 100 tries.");
            return quarterfinals;
        }


        public static List<Match> GenerateSemifinals(List<Tim> quarterfinalsWinners, Random random)
        {
            List<Match> semifinals = [];


            var hatMembership = new Dictionary<Tim, string>();  //  To see remaining teams in which hat do they belong

            foreach (var team in quarterfinalsWinners)
            {
                if (hatA.Contains(team)) hatMembership[team] = "A";
                else if (hatB.Contains(team)) hatMembership[team] = "B";
                else if (hatC.Contains(team)) hatMembership[team] = "C";
                else if (hatD.Contains(team)) hatMembership[team] = "D";
            }

            var hatAMembers = quarterfinalsWinners.Where(t => hatMembership[t] == "A").ToList();
            var hatBMembers = quarterfinalsWinners.Where(t => hatMembership[t] == "B").ToList();
            var hatCMembers = quarterfinalsWinners.Where(t => hatMembership[t] == "C").ToList();
            var hatDMembers = quarterfinalsWinners.Where(t => hatMembership[t] == "D").ToList();

            void CreateMatches(List<Tim> fromHat1, List<Tim> fromHat2)
            {
                while (fromHat1.Count > 0 && fromHat2.Count > 0 && semifinals.Count < 2)
                {
                    var team1 = fromHat1.First();
                    var team2 = fromHat2.First();
                    semifinals.Add(new Match(team1, team2));
                    fromHat1.Remove(team1);
                    fromHat2.Remove(team2);
                }
            }

            CreateMatches(hatAMembers, hatCMembers);
            CreateMatches(hatAMembers, hatDMembers);
            CreateMatches(hatBMembers, hatCMembers);
            CreateMatches(hatBMembers, hatDMembers);

            if (semifinals.Count < 2)   //  If there are no matches between hatA and hatC or hatD and same with hatB with hatC or hatD
            {
                var remainingTeams = new List<Tim>();
                remainingTeams.AddRange(hatAMembers);
                remainingTeams.AddRange(hatBMembers);
                remainingTeams.AddRange(hatCMembers);
                remainingTeams.AddRange(hatDMembers);

                Shuffle(remainingTeams, random);

                while (remainingTeams.Count >= 2 && semifinals.Count < 2)
                {
                    var team1 = remainingTeams.First();
                    var team2 = remainingTeams.Skip(1).First();
                    semifinals.Add(new Match(team1, team2));
                    remainingTeams.Remove(team1);
                    remainingTeams.Remove(team2);
                }
            }

            if (semifinals.Count < 2)
            {
                Console.WriteLine("Not enough valid pairs were created.");
            }

            return semifinals;
        }*/


        static void Shuffle<T>(List<T> list, Random random)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

    }
}
