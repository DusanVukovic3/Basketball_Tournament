using Basketball_Tournament;

List<Group> groups = Setup.LoadGroupsFromJson("groups.json");
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

var hats = Setup.GetHats(groups);

Tournament.SimulateKnockout(hats, t);




   












