using System.Text.Json;

namespace Basketball_Tournament
{
    public static class Setup
    {
        public static List<Group> InitializeGroups(string filePath)
        {
            string jsonString = File.ReadAllText(filePath);
            Dictionary<string, List<Tim>>? groupDictionary = JsonSerializer.Deserialize<Dictionary<string, List<Tim>>>(jsonString);
            List<Group> groups = groupDictionary?.Select(g => new Group(g.Key, g.Value)).ToList() ?? [];

            foreach (var group in groups)
            {
                foreach (var team in group.Teams)
                {
                    team.Group = group;  // Set the Group for each team
                }
            }

            return groups;
        }

        public static List<(int, int)[]> GetPredefinedGameSchedule()
        {
            return
        [
            [(0, 1), (2, 3)],
            [(0, 2), (1, 3)],
            [(0, 3), (1, 2)]
        ];
        }

        public static string GetFilePath()
        {
            string projectDirectory = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.FullName ?? "";
            return Path.Combine(projectDirectory, "Data", "groups.json");
        }


        public static HatsDto GetHats(List<Group> groups)
        {
            var rank1Teams = new List<Tim>();
            var rank2Teams = new List<Tim>();
            var rank3Teams = new List<Tim>();

            foreach (var g in groups)
            {
                rank1Teams.AddRange(g.Teams.Where(t => t.OverallRank == 1));
                rank2Teams.AddRange(g.Teams.Where(t => t.OverallRank == 2));
                rank3Teams.AddRange(g.Teams.Where(t => t.OverallRank == 3));
            }

            var sortedRank1Teams = rank1Teams
                .OrderByDescending(t => t.PointsInGroup)
                .ThenByDescending(t => t.PointsScored - t.PointsConceded)
                .ThenByDescending(t => t.PointsScored)
                .ToList();

            var sortedRank2Teams = rank2Teams
                .OrderByDescending(t => t.PointsInGroup)
                .ThenByDescending(t => t.PointsScored - t.PointsConceded)
                .ThenByDescending(t => t.PointsScored)
                .ToList();

            var sortedRank3Teams = rank3Teams
                .OrderByDescending(t => t.PointsInGroup)
                .ThenByDescending(t => t.PointsScored - t.PointsConceded)
                .ThenByDescending(t => t.PointsScored)
                .ToList();

            var topTeams = sortedRank1Teams
                .Concat(sortedRank2Teams)
                .Concat(sortedRank3Teams)
                .Take(8) // Top 8 teams
                .ToList();

            var hats = new HatsDto
            {
                HatA = topTeams.Take(2).ToList(),
                HatB = topTeams.Skip(2).Take(2).ToList(),
                HatC = topTeams.Skip(4).Take(2).ToList(),
                HatD = topTeams.Skip(6).Take(2).ToList()
            };

            int num2 = 1;

            Console.WriteLine("\nKnockout Phase:");
            foreach (var team in topTeams)
            {
                Console.WriteLine($"{num2}){team.Team} ");
                num2++;
            }

            return hats;
        }

    }
}
