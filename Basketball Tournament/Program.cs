using Basketball_Tournament;
using System.Text.Json;
using static System.Formats.Asn1.AsnWriter;

string projectDirectory = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.FullName ?? "";    //  Absolute path to groups.json
string filePath = Path.Combine(projectDirectory, "Data", "groups.json");
string jsonString = File.ReadAllText(filePath);

Dictionary<string, List<Tim>>? groupDictionary = JsonSerializer.Deserialize<Dictionary<string, List<Tim>>>(jsonString); //  A, B, C are keys; thats why I'm using dictionary
List<Group> groups = groupDictionary?.Select(g => new Group(g.Key, g.Value)).ToList() ?? []; // Convert the dictionary into list of groups and later write them

foreach (var group in groups)
{
    foreach (var team in group.Teams)
    {
        team.Group = group;  // Set the Group for each team
    }
}


Tim t = new();
Random random = new();
List<Match> matchesList = [];

List<(int, int)[]> legs =   // Predefined games schedule
    [
        [(0, 1), (2, 3)],
        [(0, 2), (1, 3)],
        [(0, 3), (1, 2)]
    ];

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

            double rankDifference = (double)Math.Abs(teamA.FIBARanking - teamB.FIBARanking);    // Based on difference in rank, simulate score
            double scoreDifference = rankDifference * random.NextDouble();  

            int scoreA = random.Next(60, 120) + (int)(scoreDifference / 2);  // Team will get at least 60 points and at most 120
            int scoreB = random.Next(60, 120) - (int)(scoreDifference / 2);

            while (scoreA == scoreB)
            {
                int overtimePointsTeam1 = random.Next(5, 20);   //  teams can get 5-20 points in 1 overtime, the overtime will continue until there is no longer a tie
                int overtimePointsTeam2 = random.Next(5, 20);   //  I made sure there is no more difference in FIBARanking because if teams get to overtime, that means they're pretty evenly matched

                scoreA += overtimePointsTeam1;
                scoreB += overtimePointsTeam2;

                Console.WriteLine($"\nOvertime : {teamA.Team} vs {teamB.Team} ({overtimePointsTeam1}:{overtimePointsTeam2}) ");
            }

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
foreach (var group in groups)   //  Sorting groups by points, if 2 teams same then h2h and if 3 teams same the +/-
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

    sortedTeams = [.. sortedTeams.OrderByDescending(t => t.PointsInGroup).ThenByDescending(t => t.PointsScored - t.PointsConceded).ToList()];   

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

var hatA = topTeams.Take(2).ToList();
var hatB = topTeams.Skip(2).Take(2).ToList();
var hatC = topTeams.Skip(4).Take(2).ToList();
var hatD = topTeams.Skip(6).Take(2).ToList();

int num2 = 1;
Console.WriteLine("\nKnockout Phase:");
foreach (var team in topTeams)
{
    Console.WriteLine($"{num2}){team.Team} ");
    num2++;
}


{
    Console.WriteLine("\nQUARTERFINALS:");
    var quarterfinalMatches = GenerateQuarterfinals(topTeams, random, 100);
    var quarterfinalWinners = t.SimulateRound(GenerateQuarterfinals(topTeams, random, 100));

    Console.WriteLine("\nSEMIFINALS:");
    var semifinalMatches = GenerateSemifinals(quarterfinalWinners, random); // Assuming you have a method to get semifinal matches
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


List<Match> GenerateQuarterfinals(List<Tim> topTeams, Random random, int maxRetries = 100) //  Return PAIR of teams
{
    List<Match>? quarterfinals = null;

    for (int attempt = 0; attempt < maxRetries; attempt++)
    {
        
        Shuffle(hatA, random);
        Shuffle(hatB, random);
        Shuffle(hatC, random);
        Shuffle(hatD, random);

        quarterfinals = [];

        var availableHatD = new List<Tim>(hatD);  // Copy of Hat 
        foreach (var teamA in hatA)
        {
            var potentialOpponents = availableHatD.Where(teamD => teamD.Group != teamA.Group).ToList();

            if (potentialOpponents.Count != 0)
            {
                var opponent = potentialOpponents[random.Next(potentialOpponents.Count)];
                quarterfinals.Add(new Match(teamA, opponent));
                availableHatD.Remove(opponent); // Remove selected opponent from available Hat D
            }
            else
            {
                break;  //  If we can't get valid pairs, retry
            }
        }

        var availableHatC = new List<Tim>(hatC);  
        foreach (var teamB in hatB)
        {
            var potentialOpponents = availableHatC.Where(teamC => teamC.Group != teamB.Group).ToList();

            if (potentialOpponents.Count != 0)
            {
                var opponent = potentialOpponents[random.Next(potentialOpponents.Count)];
                quarterfinals.Add(new Match(teamB, opponent));
                availableHatC.Remove(opponent); // Remove selected opponent from available Hat C
            }
            else
            {
                break;
            }
        }

        if (quarterfinals.Count == 4)   
        {
            return quarterfinals;  
        }
    }

    Console.WriteLine("Failed to generate valid quarterfinal pairs after 100 tries.");
    return quarterfinals;  
}

List<Match> GenerateSemifinals(List<Tim> quarterfinalsWinners, Random random)
{
    List<Match> semifinals = [];

    
    var hatMembership = new Dictionary<Tim, string>();  //  To see remaining teams in which hat do they belong

    foreach (var team in quarterfinalsWinners)
    {
        if (hatA.Contains(team)) hatMembership[team] = "A";
        else if (hatB.Contains(team)) hatMembership[team] = "B";
        else if (hatC.Contains(team)) hatMembership[team] = "C";
        else if (hatD.Contains(team)) hatMembership[team] = "D";
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

    if (semifinals.Count < 2)   //  If there are no matches between hatA and hatC or hatD and same with hatB with hatC or hatD
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




static void Shuffle<T>(List<T> list, Random random)
{
    for (int i = list.Count - 1; i > 0; i--)
    {
        int j = random.Next(i + 1);
        (list[i], list[j]) = (list[j], list[i]);
    }
}











