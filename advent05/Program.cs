var input = File.ReadAllLines("inputa.txt");
var lines = input.Select(i => SmokeLine.Parse(i)).ToList();

var maxX = lines.Max(l => Math.Max(l.Start.X, l.End.X));
var maxY = lines.Max(l => Math.Max(l.Start.Y, l.End.Y));

int[,] coveredPoints = new int[maxX + 1, maxY + 1];

//a
var vhPoints = lines.SelectMany(l => l.GetCoveredPoints());
foreach (var point in vhPoints)
{
    coveredPoints[point.X, point.Y]++;
}

var aResult = 0;

for (int i = 0; i <= maxX; i++)
{
    for (int j = 0; j <= maxY; j++)
    {
        if(coveredPoints[i, j] > 1)
        {
            aResult++;
        }
    }
}

Console.WriteLine(aResult);

//b
coveredPoints = new int[maxX + 1, maxY + 1];
var allPoints = lines.SelectMany(l => l.GetAllCoveredPoints());
foreach (var point in allPoints)
{
    coveredPoints[point.X, point.Y]++;
}

var bResult = 0;

for (int i = 0; i <= maxX; i++)
{
    for (int j = 0; j <= maxY; j++)
    {
        if (coveredPoints[i, j] > 1)
        {
            bResult++;
        }
    }
}

Console.WriteLine(bResult);

class SmokeLine
{
    public SmokeLine((int X, int Y) start, (int X, int Y) end)
    {
        Start = start;
        End = end;
    }

    public (int X, int Y) Start { get; set; }
    public (int X, int Y) End { get; set; }

    public static SmokeLine Parse(string input)
    {
        var parts = input.Split(" -> ", StringSplitOptions.RemoveEmptyEntries);
        return new SmokeLine(ParsePoint(parts[0]), ParsePoint(parts[1]));
    }

    private static (int X, int Y) ParsePoint(string input)
    {
        var parts = input.Split(",", StringSplitOptions.RemoveEmptyEntries);
        return (int.Parse(parts[0]), int.Parse(parts[1]));
    }

    public bool IsVertical()
    {
        return Start.X == End.X;
    }

    public bool IsHorizontal()
    {
        return Start.Y == End.Y;
    }

    public IEnumerable<(int X, int Y)> GetCoveredPoints()
    {
        if(IsHorizontal())
        {
            for (int i = Math.Min(Start.X, End.X); i <= Math.Max(Start.X, End.X); i++)
            {
                yield return (i, Start.Y);
            }
        }

        if (IsVertical())
        {
            for (int i = Math.Min(Start.Y, End.Y); i <= Math.Max(Start.Y, End.Y); i++)
            {
                yield return (Start.X, i);
            }
        }
    }

    public IEnumerable<(int X, int Y)> GetAllCoveredPoints()
    {
        var dist = Math.Max(Math.Max(Start.X, End.X) - Math.Min(Start.X, End.X), Math.Max(Start.Y, End.Y) - Math.Min(Start.Y, End.Y));
        var xDir = Math.Sign(End.X - Start.X);
        var yDir = Math.Sign(End.Y - Start.Y);
        for (int i = 0; i <= dist; i++)

        {
            yield return (Start.X + xDir * i, Start.Y + yDir * i);
        }
    }
}