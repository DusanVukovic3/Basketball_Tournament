using Basketball_Tournament;

Random random = new();
List<Match> matchesList = [];

string filePath = Setup.GetFilePath();
List<Group> groups = Setup.InitializeGroups(filePath);
List<(int, int)[]> legs = Setup.GetPredefinedGameSchedule();

Tim t = new();

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

            Tim winner = t.SimulateGame(teamA, teamB);
        }
    }
    num++;
}

Console.WriteLine("\nFinal Standings by Group:");   //  Print final group standings
var groupSorter = new GroupSorter(groups);
groupSorter.SortTeamsInGroups();

var hatsDTO = Setup.GetHats(groups);

var hatA = hatsDTO.HatA;
var hatB = hatsDTO.HatB;
var hatC = hatsDTO.HatC;
var hatD = hatsDTO.HatD;

var topTeams = hatA.Concat(hatB).Concat(hatC).Concat(hatD).ToList();

int num2 = 1;




{
    Console.WriteLine("\nQUARTERFINALS:");
    var quarterfinalMatches = GenerateQuarterfinals(topTeams, 100); //  get 4 matches
    var quarterfinalWinners = t.SimulateRound(GenerateQuarterfinals(topTeams, 100));    //  get 4 winners

    Console.WriteLine("\nSEMIFINALS:");
    var semifinalMatches = GenerateSemifinals(quarterfinalWinners, random); 
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


List<Match> GenerateQuarterfinals(List<Tim> topTeams, int maxRetries = 100)
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











