namespace Basketball_Tournament
{
    public class Tournament
    {
        private static readonly Random random = new();

        public static void SimulateKnockout(HatsDto hats, Tim t)
        {
           
            Console.WriteLine("\nQUARTERFINALS:");
            var quarterfinalMatches = GenerateQuarterfinals( hats );
            var quarterfinalWinners = t.SimulateRound(quarterfinalMatches);

            Console.WriteLine("\nSEMIFINALS:");
            var semifinalMatches = GenerateSemifinals(quarterfinalWinners, hats);
            var semifinalWinners = t.SimulateRound(semifinalMatches);

            var semifinalLosers = new List<Tim>();
            foreach (var match in semifinalMatches)
            {
                var loser = (match.Team1 == semifinalWinners[0] || match.Team1 == semifinalWinners[1]) ? match.Team2 : match.Team1;
                semifinalLosers.Add(loser);
            }

            Console.WriteLine("\n3RD PLACE MATCH:");
            var thirdPlaceMatch = new Match(semifinalLosers[0], semifinalLosers[1]);
            var thirdPlaceWinner = t.SimulateGame(thirdPlaceMatch.Team1, thirdPlaceMatch.Team2);

            Console.WriteLine("\nFINAL:");
            var finalMatch = new Match(semifinalWinners[0], semifinalWinners[1]);
            var champion = t.SimulateGame(finalMatch.Team1, finalMatch.Team2);
            var finalLoser = (semifinalWinners[0] == champion) ? semifinalWinners[1] : semifinalWinners[0];

            Console.WriteLine($"\nGold Medal: {champion.Team}");
            Console.WriteLine($"Silver Medal: {finalLoser.Team}");
            Console.WriteLine($"Bronze Medal: {thirdPlaceWinner.Team}");
        }

        static List<Match> GenerateQuarterfinals(HatsDto dto)
        {
            List<Match> quarterfinals = [];

            for (int attempt = 0; attempt < 100; attempt++)
            {
                var shuffledHatA = dto.HatA.OrderBy(t => random.Next()).ToList();
                var shuffledHatB = dto.HatB.OrderBy(t => random.Next()).ToList();
                var shuffledHatC = dto.HatC.OrderBy(t => random.Next()).ToList();
                var shuffledHatD = dto.HatD.OrderBy(t => random.Next()).ToList();

                var availableHatD = new List<Tim>(shuffledHatD);
                var availableHatC = new List<Tim>(shuffledHatC);

                quarterfinals.Clear();

                var hatAAndDMatches = new List<Match>();
                for (int i = 0; i < shuffledHatA.Count; i++)
                {
                    var teamA = shuffledHatA[i];
                    var teamD = shuffledHatD[i];
                    hatAAndDMatches.Add(new Match(teamA, teamD));
                }

                var hatBAndCMatches = new List<Match>();
                for (int i = 0; i < shuffledHatB.Count; i++)
                {
                    var teamB = shuffledHatB[i];
                    var teamC = shuffledHatC[i];
                    hatBAndCMatches.Add(new Match(teamB, teamC));
                }

                quarterfinals.AddRange(hatAAndDMatches);
                quarterfinals.AddRange(hatBAndCMatches);

                if (quarterfinals.Count == 4)
                {
                    return quarterfinals; 
                }
            }

            Console.WriteLine("Failed to generate valid quarterfinal pairs after 100 tries.");
            return quarterfinals; 
        }



        public static List<Match> GenerateSemifinals(List<Tim> quarterfinalsWinners, HatsDto hatsDTO)
        {
            List<Match> semifinals = [];

            var hatMembership = new Dictionary<Tim, string>();  // To see remaining teams in which hat do they belong

            foreach (var team in quarterfinalsWinners)
            {
                if (hatsDTO.HatA.Contains(team)) hatMembership[team] = "A";
                else if (hatsDTO.HatB.Contains(team)) hatMembership[team] = "B";
                else if (hatsDTO.HatC.Contains(team)) hatMembership[team] = "C";
                else if (hatsDTO.HatD.Contains(team)) hatMembership[team] = "D";
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

            if (semifinals.Count < 2)   // If there are no matches between hatA and hatC or hatD and same with hatB with hatC or hatD
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
        }

        private static void Shuffle<T>(List<T> list, Random random)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}
