namespace Basketball_Tournament
{
    public class GroupSorter
    {
        private readonly List<Group> _groups;
        private readonly Dictionary<(Tim, Tim), Match> _matchDictionary = [];

        public GroupSorter(List<Group> groups)
        {
            _groups = groups;
            InitializeMatchDictionary();
        }

        private void InitializeMatchDictionary()
        {
            foreach (var group in _groups)
            {
                foreach (var match in group.Matches)
                {
                    _matchDictionary[(match.Team1, match.Team2)] = match;
                    _matchDictionary[(match.Team2, match.Team1)] = match;
                }
            }
        }

        public void SortTeamsInGroups()
        {
            foreach (var group in _groups)
            {
                Console.WriteLine($"\nGroup {group.GroupName}:");

                var sortedTeams = group.Teams
                    .OrderByDescending(t => t.PointsInGroup)
                    .ToList();

                ResolveTies(sortedTeams);

                sortedTeams = [.. sortedTeams
                    .OrderByDescending(t => t.PointsInGroup)
                    .ThenByDescending(t => t.PointsScored - t.PointsConceded)];

                AssignRanks(sortedTeams);

                PrintTeamDetails(sortedTeams);
            }
        }

        private void ResolveTies(List<Tim> sortedTeams)
        {
            for (int i = 0; i < sortedTeams.Count - 1; i++)
            {
                for (int j = i + 1; j < sortedTeams.Count; j++)
                {
                    if (sortedTeams[i].PointsInGroup == sortedTeams[j].PointsInGroup)
                    {
                        var match = _matchDictionary.TryGetValue((sortedTeams[i], sortedTeams[j]), out var foundMatch) ? foundMatch :
                                    _matchDictionary.TryGetValue((sortedTeams[j], sortedTeams[i]), out foundMatch) ? foundMatch : null;

                        if (match != null)
                        {
                            var winner = match.GetWinner();
                            if (winner == sortedTeams[j])
                            {
                                (sortedTeams[i], sortedTeams[j]) = (sortedTeams[j], sortedTeams[i]);
                            }
                        }
                    }
                }
            }
        }

        private static void AssignRanks(List<Tim> sortedTeams)
        {
            int rank = 1;
            foreach (var team in sortedTeams)
            {
                team.OverallRank = rank++;
            }
        }

        private static void PrintTeamDetails(List<Tim> sortedTeams)
        {
            foreach (var team in sortedTeams)
            {
                Console.WriteLine($"{team.OverallRank}) {team.Team}: Pts: {team.PointsInGroup} | PointsScored: {team.PointsScored} | PointsConceded: {team.PointsConceded} | +/-: {team.PointsScored - team.PointsConceded}");
            }
        }
    }
}

