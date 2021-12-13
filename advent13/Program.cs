var inputString = File.ReadAllText("inputa.txt");

var parts = inputString.Split("\r\n\r\n");
var coordinates = parts[0].Split("\r\n").Select(line =>
{
    var lineSplit = line.Split(",");
    return (X: int.Parse(lineSplit[0]), Y: int.Parse(lineSplit[1]));
}).ToHashSet();

var folds = parts[1].Split("\r\n").Select(line =>
{
    var lineSplit = line.Split("=");
    return (Axis: lineSplit[0].Last(), Value: int.Parse(lineSplit[1]));
}).ToList();


foreach (var fold in folds.Take(1))
{
    coordinates = FoldSimple(coordinates, fold.Axis, fold.Value);
}
//a
Console.WriteLine(coordinates.Count);

foreach (var fold in folds.Skip(1))
{
    coordinates = FoldSimple(coordinates, fold.Axis, fold.Value);
}

//b
PrintCoords(coordinates);



HashSet<(int X, int Y)> FoldSimple(HashSet<(int X, int Y)> coordinates, char axis, int value)
{
    var result = new HashSet<(int X, int Y)>();
    if(axis == 'x')
    {
        foreach(var coord in coordinates)
        {
            if(coord.X > value)
            {
                result.Add((coord.X - 2 * (coord.X - value), coord.Y));
            } 
            else
            {
                result.Add(coord);
            }
        }
    }
    else
    {
        foreach (var coord in coordinates)
        {
            if (coord.Y > value)
            {
                result.Add((coord.X, coord.Y - 2 * (coord.Y - value)));
            }
            else
            {
                result.Add(coord);
            }
        }
    }

    return result;
}

void PrintCoords(HashSet<(int X, int Y)> coordinates)
{
    var minX = coordinates.Min(c => c.X);
    var maxX = coordinates.Max(c => c.X);
    var minY = coordinates.Min(c => c.Y);
    var maxY = coordinates.Max(c => c.Y);

    for (int j = minY; j <= maxY; j++)
    {
        for (int i = minX; i <= maxX; i++)
        {
            if(coordinates.Contains((i, j)))
            {
                Console.Write('#');
            }
            else
            {
                Console.Write('.');
            }
        }

        Console.WriteLine();
    }
}

