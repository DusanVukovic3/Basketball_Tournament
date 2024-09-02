namespace Basketball_Tournament
{
    public class GroupSorter(List<Group> groups)
    {
        private readonly List<Group> _groups = groups;

        public void SortTeamsInGroups()
        {
            foreach (var group in _groups)
            {
                Console.WriteLine($"\nGroup {group.GroupName}:");

                var sortedTeams = group.Teams   //  Sort by points
                    .OrderByDescending(t => t.PointsInGroup)
                    .ToList();

                sortedTeams = ResolveTies(sortedTeams);

                AssignRanks(sortedTeams);  

                PrintTeamDetails(sortedTeams);
            }
        }

        private static List<Tim> ResolveTies(List<Tim> sortedTeams)
        {
            int i = 0;

            while (i < sortedTeams.Count - 1)
            {
                int j = i + 1;

                while (j < sortedTeams.Count && sortedTeams[i].PointsInGroup == sortedTeams[j].PointsInGroup)
                {
                    j++;
                }

                if (j - i == 2) // Two teams tie
                {
                    var teamA = sortedTeams[i];
                    var teamB = sortedTeams[i + 1];

                    var match = FindHeadToHeadMatch(teamA, teamB);


                    if (match != null)
                    {
                        var winner = match.GetWinner();

                        if (winner == teamB)
                        {
                            (sortedTeams[i], sortedTeams[i + 1]) = (sortedTeams[i + 1], sortedTeams[i]);
                        }
                    }
                    else
                    {

                        if (sortedTeams[i].PointsScored - sortedTeams[i].PointsConceded <
                            sortedTeams[i + 1].PointsScored - sortedTeams[i + 1].PointsConceded)
                        {
 
                            (sortedTeams[i], sortedTeams[i + 1]) = (sortedTeams[i + 1], sortedTeams[i]);
                        }
                    }
                }
                else if (j - i > 2) //  Three teams tie
                {
                    
                    var subList = sortedTeams.GetRange(i, j - i)
                        .OrderByDescending(t => t.PointsScored - t.PointsConceded)
                        .ToList();


                    sortedTeams.RemoveRange(i, j - i);
                    sortedTeams.InsertRange(i, subList);
                }

                i = j; 
            }

            return sortedTeams;
        }

        private static Match? FindHeadToHeadMatch(Tim teamA, Tim teamB)
        {
            var match = teamA.MatchesList.FirstOrDefault(m =>
                (m.Team1 == teamA && m.Team2 == teamB) || (m.Team1 == teamB && m.Team2 == teamA));

            if (match == null)
            {
                Console.WriteLine($"No match found for {teamA.Team} vs {teamB.Team}");
            }

            return match;
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
