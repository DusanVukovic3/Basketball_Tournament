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



    }
}
