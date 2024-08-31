using Basketball_Tournament;
using System.Text.Json;

string projectDirectory = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.FullName ?? "";    //  Absolute path to groups.json
string filePath = Path.Combine(projectDirectory, "Data", "groups.json");
string jsonString = File.ReadAllText(filePath);

Dictionary<string, List<Tim>>? groupDictionary = JsonSerializer.Deserialize<Dictionary<string, List<Tim>>>(jsonString); //  A, B, C are keys; this will work if more groups are added 
List<Group> groups = groupDictionary?.Select(g => new Group(g.Key, g.Value)).ToList() ?? []; // Convert the dictionary into list of groups and later write them



Random random = new();
List<Match> matchesList = [];

List<(int, int)[]> legs =   // Predefined games schedule
    [
        [(0, 1), (2, 3)],
        [(0, 2), (1, 3)],
        [(0, 3), (1, 2)]
    ];

//Dictionary<string, Dictionary<string, int>> groupPoints = [];   // Store sum of points

foreach (Group group in groups)
{
    matchesList.AddRange(group.Matches);
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

            Match match = new(teamA, teamB);    //  Just after simulating match do we have a list with some elements   
            match.SetResult(scoreA, scoreB);
            matchesList.Add(match);

            group.Matches.Add(match);


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

    var matchDictionary = group.Matches.ToDictionary(
        m => (m.Team1, m.Team2),
        m => m
    );

    var sortedTeams = group.Teams
        .OrderByDescending(t => t.PointsInGroup).ToList();

    for (int i = 0; i < sortedTeams.Count - 1; i++)
    {
        for (int j = i + 1; j < sortedTeams.Count; j++) //  For every team, go through all the opponents
        {
            if (sortedTeams[i].PointsInGroup == sortedTeams[j].PointsInGroup)
            {
                var match = matchDictionary.TryGetValue((sortedTeams[i], sortedTeams[j]), out var foundMatch) ? foundMatch :
                            matchDictionary.TryGetValue((sortedTeams[j], sortedTeams[i]), out foundMatch) ? foundMatch : null;

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

    sortedTeams = [.. sortedTeams.OrderByDescending(t => t.PointsInGroup).ThenByDescending(t => t.PointsScored - t.PointsConceded).ToList()];   //  sorting by +/-

    int rank = 1;
    foreach(var team in sortedTeams)
    {
        team.OverallRank = rank++;
    }

    foreach (var team in sortedTeams)
    {
        Console.WriteLine($"{team.OverallRank}) {team.Team}: Pts : {team.PointsInGroup} | PointsScored : {team.PointsScored} | PointsConceded : {team.PointsConceded} | +/- : {team.PointsScored - team.PointsConceded} "); 
    }

    
}

var rank1Teams = new List<Tim>();
var rank2Teams = new List<Tim>();
var rank3Teams = new List<Tim>();


foreach (var g in groups)
{
    rank1Teams.AddRange(g.Teams.Where(t => t.OverallRank == 1));    //  Collect teams by rank from all groups
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
.Take(8) // Select only the top 8 teams
.ToList();

int num2 = 1;
Console.WriteLine("\nKnockout Phase:");
foreach (var team in topTeams)
{
    Console.WriteLine($"{num2}){team.Team} ");
    num2++;
}



var random2 = new Random();

var hatA = topTeams.Take(2).ToList(); 
var hatB = topTeams.Skip(2).Take(2).ToList();
var hatC = topTeams.Skip(4).Take(2).ToList();
var hatD = topTeams.Skip(6).Take(2).ToList();

Shuffle(hatA, random2);
Shuffle(hatB, random2);
Shuffle(hatC, random2);
Shuffle(hatD, random2);

var quarterfinals = new List<(Tim, Tim)>();

foreach (var teamA in hatA)
{
    var potentialOpponents = hatD.Where(teamD => teamD.Group != teamA.Group).ToList();

    if (potentialOpponents.Count != 0)
    {
        var opponent = potentialOpponents[random.Next(potentialOpponents.Count)];
        quarterfinals.Add((teamA, opponent));
        hatD.Remove(opponent); // Remove selected opponent from Hat D
    }
}

foreach (var teamB in hatB)
{
    var potentialOpponents = hatC.Where(teamC => teamC.Group != teamB.Group).ToList();

    if (potentialOpponents.Count != 0)
    {
        var opponent = potentialOpponents[random.Next(potentialOpponents.Count)];
        quarterfinals.Add((teamB, opponent));
        hatC.Remove(opponent); // Remove selected opponent from Hat C
    }
}




Tim t = new();

{
    Console.WriteLine("\nQUARTERFINALS:");
    var quarterfinalWinners = t.SimulateRound(quarterfinals);

    Console.WriteLine("\nSEMIFINALS:");
    var semifinals = new List<(Tim, Tim)>
{
    (quarterfinalWinners[0], quarterfinalWinners[1]),  // Winner of QF1 vs Winner of QF2
    (quarterfinalWinners[2], quarterfinalWinners[3])   // Winner of QF3 vs Winner of QF4

    
};
    var semifinalWinners = t.SimulateRound(semifinals);

    Console.WriteLine("\nFINAL:");
    var finalMatch = (semifinalWinners[0], semifinalWinners[1]);
    var champion = t.SimulateGame(finalMatch.Item1, finalMatch.Item2);

    Console.WriteLine($"\nCHAMPION: \n{champion.Team} ");
}





static void Shuffle<T>(List<T> list, Random random)
{
    for (int i = list.Count - 1; i > 0; i--)
    {
        int j = random.Next(i + 1);
        (list[i], list[j]) = (list[j], list[i]);
    }
}











