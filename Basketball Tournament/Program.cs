using Basketball_Tournament;
using System;
using System.Text.Json;

string projectDirectory = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.FullName ?? "";    //  Absolute path to groups.json
string filePath = Path.Combine(projectDirectory, "Data", "groups.json");
string jsonString = File.ReadAllText(filePath);

Dictionary<string, List<Tim>>? groupDictionary = JsonSerializer.Deserialize<Dictionary<string, List<Tim>>>(jsonString); //  A, B, C are keys; this will work if more groups are added 
List<Group> groups = groupDictionary?.Select(g => new Group(g.Key, g.Value)).ToList() ?? []; // Convert the dictionary into list of groups and later write them



Random random = new();
List<Tim> teamsList = [];

List<(int, int)[]> legs =   // Predefined games schedule
    [
        [(0, 1), (2, 3)],
        [(0, 2), (1, 3)],
        [(0, 3), (1, 2)]
    ];

//Dictionary<string, Dictionary<string, int>> groupPoints = [];   // Store sum of points

foreach (Group group in groups)
{
    teamsList.AddRange(group.Teams);
}

int num = 1;

foreach (var leg in legs)   // Simulate each leg
{
    Console.WriteLine($"\n{num}. Leg:");

    foreach (Group group in groups)
    {
        Console.WriteLine($"\nGroup: {group.GroupName}");
        var teams = group.Teams;

        foreach (var (i, j) in leg)
        {
            var teamA = teams[i];
            var teamB = teams[j];

            /*double probabilityTeamAWins = (double)teamB.FIBARanking / (teamA.FIBARanking + teamB.FIBARanking);
            double randomValue = random.NextDouble();   // 0.0 - 1.0

            string winner = randomValue < probabilityTeamAWins ? teamA.Team : teamB.Team;*/

            double rankDifference = (double)Math.Abs(teamA.FIBARanking - teamB.FIBARanking);    // Based on difference in rank, simulate score
            double scoreDifference = rankDifference * random.NextDouble();  

            int scoreA = random.Next(60, 120) + (int)(scoreDifference / 2);  // Team will get at least 60 points and at most 120
            int scoreB = random.Next(60, 120) - (int)(scoreDifference / 2);  

            Console.WriteLine($"{teamA.Team} vs {teamB.Team} ({scoreA}:{scoreB})");

            if (scoreA > scoreB)
            {
                teamA.PointsInGroup += 2;    // Group points
                teamB.PointsInGroup += 1;
            }
            else
            {
                teamA.PointsInGroup += 1;    
                teamB.PointsInGroup += 2;
            }

            teamA.PointsScored += scoreA;
            teamA.PointsConceded += scoreB;
            teamB.PointsScored += scoreB;
            teamB.PointsConceded += scoreA;
            
        }
    }
    num++;
}





Console.WriteLine("\nFinal Standings by Group:");

foreach (var group in groups)
{
    Console.WriteLine($"\nGroup {group.GroupName}:");

    var sortedTeams = group.Teams
        .OrderByDescending(t => t.PointsInGroup)
        .ThenByDescending(t => t.PointsScored - t.PointsConceded);  // If same PointsInGroup, sort by +/-

    int num1 = 1;

    foreach (var team in sortedTeams)
    {
        
        int pointDifferential = team.PointsScored - team.PointsConceded;
        Console.WriteLine($"{num1}) {team.Team}: Pts : {team.PointsInGroup} | PointsScored : {team.PointsScored} | PointsConceded : {team.PointsConceded} | +/- : {pointDifferential}");
        num1++;
    }
    
}





