using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basketball_Tournament
{
    public class GameSimulator
    {
        private Random _random;

        public GameSimulator(Random random)
        {
            _random = random;
        }

        public Match SimulateGame(Tim team1, Tim team2)
        {
            double rankDifference = (double)Math.Abs(team1.FIBARanking - team2.FIBARanking);
            double scoreDifference = rankDifference * _random.NextDouble();

            int scoreA = _random.Next(60, 120) + (int)(scoreDifference / 2);
            int scoreB = _random.Next(60, 120) - (int)(scoreDifference / 2);

            while (scoreA == scoreB)
            {
                int overtimePointsTeam1 = _random.Next(5, 20);
                int overtimePointsTeam2 = _random.Next(5, 20);

                scoreA += overtimePointsTeam1;
                scoreB += overtimePointsTeam2;
            }

            var match = new Match(team1, team2);
            match.SetResult(scoreA, scoreB);

            return match;
        }

        public List<Match> SimulateRound(List<Match> matches)
        {
            return matches.Select(match => SimulateGame(match.Team1, match.Team2)).ToList();
        }
    }

}
