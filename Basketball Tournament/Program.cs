using Basketball_Tournament;
using System.Text.Json;

string projectDirectory = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.FullName ?? "";    //Absolute path to groups.json
string filePath = Path.Combine(projectDirectory, "Data", "groups.json");
string jsonString = File.ReadAllText(filePath);

Dictionary<string, List<Tim>>? groupDictionary = JsonSerializer.Deserialize<Dictionary<string, List<Tim>>>(jsonString); //Using dictionary because in .json we have dictionary, A, B, C are keys; this will work if more groups are added 

List<Group> groups = groupDictionary?.Select(g => new Group(g.Key, g.Value)).ToList() ?? []; // Convert the dictionary into list of groups and later write them

foreach (var g in groups)
{
    Console.WriteLine($"Group: {g.GroupName}");
    foreach (var team in g.Teams)
    {
        Console.WriteLine($"  Team: {team.Team}, ISOCode: {team.ISOCode}, FIBARanking: {team.FIBARanking}");
    }
    Console.WriteLine(); 
}


Random random = new();  

foreach (var group in groups)
{
    Console.WriteLine($"Simulating matches for Group: {group.GroupName}");
    var teams = group.Teams;

    Dictionary<string, int> teamPoints = teams.ToDictionary(t => t.Team, t => 0);   //  So we don't need to add aditional field in Team.cs called points; we re using this points just to see how the teams will match in knockout phase

    for (int i = 0; i < teams.Count; i++)   // Each team plays 3 others in the group
    {
        for (int j = i + 1; j < teams.Count; j++)
        {
            var teamA = teams[i];
            var teamB = teams[j];

          
            double probabilityTeamAWins = (double)teamB.FIBARanking / (teamA.FIBARanking + teamB.FIBARanking);  // Formula for probability -> P(a) = Rank(b) / (Rank(b) + Rank(a)), better than just straight difference between ranks because then it seems like 1vs8 is the same as 20vs27
            double probabilityTeamBWins = (double)teamA.FIBARanking / (teamA.FIBARanking + teamB.FIBARanking);  //I wanted the gap to be bigger between 1st and 8th team as opposed to 20th and 27th team

            double randomValue = random.NextDouble();   // 0.0 - 1.0

            string winner = randomValue < probabilityTeamAWins ? teamA.Team : teamB.Team;   //Random number 0.0 - 1.0 but it's not totally random, it's probabilityTeamAWins * randomNumber 

            if (winner == teamA.Team)   //  add points after every match
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

    var sortedStandings = teamPoints.OrderByDescending(tp => tp.Value);

    Console.WriteLine($"\nGroup {group.GroupName}:");
    foreach (var team in sortedStandings)
    {
        Console.WriteLine($"{team.Key}: {team.Value} points");
    }

    Console.WriteLine();
}
