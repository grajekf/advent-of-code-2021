var connections = File.ReadAllLines("inputa.txt");
var caveSystem = CaveSystem.Parse(connections);

//a
Console.WriteLine(caveSystem.GetUniqueRoutes().Count());
//b
Console.WriteLine(caveSystem.GetUniqueRoutesSmallTwice().Count());


class CaveSystem
{
    public CaveSystem(IEnumerable<Cave> caves)
    {
        Caves = caves;
        StartCave = caves.First(c => c.IsStart());
        EndCave = caves.First(c => c.IsEnd());
    }

    public static CaveSystem Parse(IEnumerable<string> connections)
    {
        var caves = new HashSet<Cave>();
        foreach(var connection in connections)
        {
            var parts = connection.Split("-");
            var fromName = parts[0];
            var toName = parts[1];
            var fromCave = caves.FirstOrDefault(c => c.Name == fromName) ?? new Cave(fromName);
            var toCave = caves.FirstOrDefault(c => c.Name == toName) ?? new Cave(toName);
            

            fromCave.Connected.Add(toCave);
            toCave.Connected.Add(fromCave);

            caves.Add(fromCave);
            caves.Add(toCave);
        }

        return new CaveSystem(caves.ToList());
    }

    public IEnumerable<Route> GetUniqueRoutes()
    {
        var visitedSmallCaves = new HashSet<Cave>();
        var currentRoute = new List<Cave>();

        currentRoute.Add(StartCave);
        visitedSmallCaves.Add(StartCave);

        return GetUniqueRoutesRec(currentRoute, visitedSmallCaves);

    }

    private HashSet<Route> GetUniqueRoutesRec(IList<Cave> currentRoute, HashSet<Cave> visitedSmallCaves)
    {
        var lastNode = currentRoute.Last();
        if (lastNode.IsEnd())
        {
            return new HashSet<Route>(new List<Route> { new Route(currentRoute) });
        }

        var routes = new HashSet<Route>();

        foreach(var connected in lastNode.Connected)
        {
            if(!visitedSmallCaves.Contains(connected))
            {
                var newRoute = new List<Cave>(currentRoute)
                {
                    connected
                };
                if (connected.IsSmall())
                {
                    visitedSmallCaves.Add(connected);
                }

                routes.UnionWith(GetUniqueRoutesRec(newRoute, visitedSmallCaves));

                visitedSmallCaves.Remove(connected);
            }
        }

        return routes;
    }

    public IEnumerable<Route> GetUniqueRoutesSmallTwice()
    {
        var visitedSmallCaves = new HashSet<Cave>();
        var currentRoute = new List<Cave>();

        currentRoute.Add(StartCave);
        visitedSmallCaves.Add(StartCave);

        var cavesToDoubleVisit = Caves.Where(x => x.IsSmall() && !x.IsEnd() && !x.IsStart()).ToList();

        HashSet<Route> routes = new HashSet<Route>();

        foreach (var caveToDoubleVisit in cavesToDoubleVisit)
        {
            routes.UnionWith(GetUniqueRoutesRecSmallTwice(currentRoute, visitedSmallCaves, caveToDoubleVisit, false));
        }

        return routes;
    }

    private HashSet<Route> GetUniqueRoutesRecSmallTwice(IList<Cave> currentRoute, HashSet<Cave> visitedSmallCaves, Cave doubleVisitCave, bool didDoubleVisit)
    {
        var lastNode = currentRoute.Last();
        if (lastNode.IsEnd())
        {
            return new HashSet<Route>(new List<Route> { new Route(currentRoute) });
        }

        var routes = new HashSet<Route>();

        foreach (var connected in lastNode.Connected)
        {
            var doDoubleVisit = visitedSmallCaves.Contains(connected) && !didDoubleVisit && doubleVisitCave == connected;
            if (!visitedSmallCaves.Contains(connected) || doDoubleVisit)
            {
                var newRoute = new List<Cave>(currentRoute)
                {
                    connected
                };
                if (connected.IsSmall())
                {
                    visitedSmallCaves.Add(connected);
                }

                if(doDoubleVisit)
                {
                    didDoubleVisit = true;
                }

                routes.UnionWith(GetUniqueRoutesRecSmallTwice(newRoute, visitedSmallCaves, doubleVisitCave, didDoubleVisit));

                if (!doDoubleVisit)
                {
                    visitedSmallCaves.Remove(connected);
                }
                else
                {
                    didDoubleVisit = false;
                }
            }
        }

        return routes;
    }

    public IEnumerable<Cave> Caves { get; set; }
    public Cave StartCave { get; set; }
    public Cave EndCave { get; set; }
}


class Cave
{
    public Cave(string name)
    {
        Name = name;
        Connected = new List<Cave>();
    }

    public string Name { get; set; }

    public IList<Cave> Connected { get; set; }
    public bool IsSmall()
    {
        return !IsEnd() && Name.All(x => char.IsLower(x));
    }

    public bool IsStart()
    {
        return Name == "start";
    }

    public bool IsEnd()
    {
        return Name == "end";
    }

    // override object.Equals
    public override bool Equals(object obj)
    {
        //       
        // See the full list of guidelines at
        //   http://go.microsoft.com/fwlink/?LinkID=85237  
        // and also the guidance for operator== at
        //   http://go.microsoft.com/fwlink/?LinkId=85238
        //

        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        // TODO: write your implementation of Equals() here
        return Name.Equals(((Cave)obj).Name);
    }

    // override object.GetHashCode
    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
}

class Route
{
    public Route(IEnumerable<Cave> visitedCaves)
    {
        VisitedCaves = visitedCaves;
        Name = string.Join("->", VisitedCaves.Select(x => x.Name));
    }

    public IEnumerable<Cave> VisitedCaves { get; set; }
    public string Name { get; set; }

    // override object.Equals
    public override bool Equals(object obj)
    {
        //       
        // See the full list of guidelines at
        //   http://go.microsoft.com/fwlink/?LinkID=85237  
        // and also the guidance for operator== at
        //   http://go.microsoft.com/fwlink/?LinkId=85238
        //

        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        return Name.Equals(((Route)obj).Name);
    }

    // override object.GetHashCode
    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
}