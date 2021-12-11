var input = File.ReadAllLines("inputa.txt");

var columnCount = input.Length;
var rowCount = input.First().Length;

var octopuses = new int[columnCount, rowCount];

for (int i = 0; i < columnCount; i++)
{
    for (int j = 0; j < rowCount; j++)
    {
        octopuses[i, j] = int.Parse(input[i][j].ToString());
    }
}

const int stepCount = 100;
var flashCount = 0;

for (int step = 0; true; step++)
{
    for (int i = 0; i < columnCount; i++)
    {
        for (int j = 0; j < rowCount; j++)
        {
            octopuses[i, j]++;
        }
    }


    bool change = true;
    HashSet<(int X, int Y)> flashes = new HashSet<(int X, int Y)>();

    while(change)
    {
        change = false;
        for (int i = 0; i < columnCount; i++)
        {
            for (int j = 0; j < rowCount; j++)
            {
                if(octopuses[i, j] > 9 && !flashes.Contains((i, j)))
                {
                    change = true;
                    flashes.Add((i, j));
                    foreach(var neighbour in GetNeighbours(octopuses, i, j))
                    {
                        octopuses[neighbour.X, neighbour.Y]++;
                    }
                }
            }
        }
    }

    flashCount += flashes.Count;

    //a
    if(step == stepCount - 1)
    {
        Console.WriteLine(flashCount);
    }
    
    //b
    if (flashes.Count == columnCount * rowCount)
    {
        Console.WriteLine(step + 1);
        break;
    }

    foreach(var flash in flashes)
    {
        octopuses[flash.X, flash.Y] = 0;
    }
}

IEnumerable<(int X, int Y)> GetNeighbours(int[,] map, int x, int y)
{
    for(int i = Math.Max(0, x - 1); i <= Math.Min(x + 1, map.GetLength(0) - 1); i++)
    {
        for (int j = Math.Max(0, y - 1); j <= Math.Min(y + 1, map.GetLength(1) - 1); j++)
        {
            if(i != x || j != y)
            {
                yield return (i, j);
            }
        }
    }
}