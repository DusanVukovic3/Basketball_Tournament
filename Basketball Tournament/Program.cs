using Basketball_Tournament;
using System.Text.Json;

string projectDirectory = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.FullName ?? "";    //  Absolute path to groups.json
string filePath = Path.Combine(projectDirectory, "Data", "groups.json");
string jsonString = File.ReadAllText(filePath);

Dictionary<string, List<Tim>>? groupDictionary = JsonSerializer.Deserialize<Dictionary<string, List<Tim>>>(jsonString); //  A, B, C are keys; this will work if more groups are added 

List<Group> groups = groupDictionary?.Select(g => new Group(g.Key, g.Value)).ToList() ?? []; // Convert the dictionary into list of groups and later write them

/*foreach (var g in groups)
{
    Console.WriteLine($"Group: {g.GroupName}");
    foreach (var team in g.Teams)
    {
        Console.WriteLine($"  Team: {team.Team}, ISOCode: {team.ISOCode}, FIBARanking: {team.FIBARanking}");
    }
    Console.WriteLine(); 
}*/


Random random = new(); 

List<(int, int)[]> legs =   //  predefined games schedule -> 1st leg is 1st vs 2nd and 3rd vs 4th, 2nd leg is 1st vs 3rd and 2nd vs 4th 
    [
        [(0, 1), (2, 3)],
        [(0, 2), (1, 3)],
        [(0, 3), (1, 2)]
    ];

Dictionary<string, Dictionary<string, int>> groupPoints = [];   //  We put sum of points here

foreach (var group in groups)
{
    groupPoints[group.GroupName] = group.Teams.ToDictionary(t => t.Team, t => 0);   
}

int num = 1;

foreach (var leg in legs)   // Each team plays every other in group, 3 total games per team
{
    Console.WriteLine($"\n{num}. Leg:");

    foreach (var group in groups)
    {
        Console.WriteLine($"\nGroup: {group.GroupName}");
        var teams = group.Teams;
        var teamPoints = groupPoints[group.GroupName];

        foreach (var (i, j) in leg)
        {
            var teamA = teams[i];
            var teamB = teams[j];

            double probabilityTeamAWins = (double)teamB.FIBARanking / (teamA.FIBARanking + teamB.FIBARanking);  // Probability calculation
            double randomValue = random.NextDouble();   // 0.0 - 1.0

            string winner = randomValue < probabilityTeamAWins ? teamA.Team : teamB.Team;

            if (winner == teamA.Team)   // Add points after every match
            {
                teamPoints[teamA.Team] += 2;
                teamPoints[teamB.Team] += 1;
            }
            else
            {
                teamPoints[teamA.Team] += 1;
                teamPoints[teamB.Team] += 2;
            }

            Console.WriteLine($"{teamA.Team} vs {teamB.Team}: Winner is {winner}");
        }
    }

    num++;

    

}

foreach (var group in groups)
{
    var teamPoints = groupPoints[group.GroupName]; 
    var sortedStandings = teamPoints.OrderByDescending(tp => tp.Value);

    Console.WriteLine($"\nFinal Standings for Group {group.GroupName}:");
    foreach (var team in sortedStandings)
    {
        Console.WriteLine($"{team.Key}: {team.Value} points");
    }
    Console.WriteLine();  
}

