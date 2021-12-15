var input = File.ReadAllLines("inputa.txt");

var columnCount = input.Length;
var rowCount = input.First().Length;

var cave = new int[columnCount, rowCount];

for (int i = 0; i < columnCount; i++)
{
    for (int j = 0; j < rowCount; j++)
    {
        cave[i, j] = int.Parse(input[i][j].ToString());
    }
}

(int X, int Y) start = (0, 0);
//a
(int X, int Y) end = (columnCount - 1, rowCount - 1);
Console.WriteLine(GetRisk(cave, start, end));

//b
var expandedCave = new int[columnCount * 5, rowCount * 5];
for(int xRepeat = 0; xRepeat < 5; xRepeat++)
{
    for (int yRepeat = 0; yRepeat < 5; yRepeat++)
    {
        var increase = xRepeat + yRepeat;
        for (int i = 0; i < columnCount; i++)
        {
            for (int j = 0; j < rowCount; j++)
            {
                var newValue = cave[i, j] + increase;
                if(newValue > 9)
                {
                    newValue -= 9;
                }
                expandedCave[i + columnCount * xRepeat, j + rowCount * yRepeat] = newValue;
            }
        }
    }
}

var newEnd = (columnCount * 5 - 1, rowCount * 5 - 1);

Console.WriteLine(GetRisk(expandedCave, start, newEnd));



int GetRisk(int[,] cave, (int X, int Y) start, (int X, int Y) end)
{
    var visited = new HashSet<(int X, int Y)>();
    var openTiles = new HashSet<(int X, int Y)>();
    var priorities = new Dictionary<(int X, int Y), int>();
    var distancedFromStart = new Dictionary<(int X, int Y), int>();
    distancedFromStart[start] = 0;
    openTiles.Add(start);
    priorities[start] = ManhattanDistance(start, end);

    while (openTiles.Count > 0)
    {
        var tile = priorities.MinBy(p => p.Value).Key;
        if (tile == end)
        {
            return distancedFromStart[end];
        }

        visited.Add(tile);
        openTiles.Remove(tile);
        priorities.Remove(tile);

        foreach (var neighbour in GetNeighbours(cave, tile.X, tile.Y).Where(n => !visited.Contains(n)))
        {
            var newDist = distancedFromStart[tile] + cave[neighbour.X, neighbour.Y];
            bool isBetter = false;

            if (!distancedFromStart.ContainsKey(neighbour))
            {
                distancedFromStart[neighbour] = 100000;
            }

            if (!openTiles.Contains(neighbour))
            {
                openTiles.Add(neighbour);
                isBetter = true;
            }
            else if (newDist < distancedFromStart[neighbour])
            {
                isBetter = true;
            }

            if (isBetter)
            {
                distancedFromStart[neighbour] = newDist;
                priorities[neighbour] = newDist + ManhattanDistance(neighbour, end);
            }
        }
    }

    return int.MaxValue;
}

int ManhattanDistance((int X, int Y) from, (int X, int Y) to)
{
    return Math.Abs(from.X - to.X) + Math.Abs(from.Y - to.Y);
}

IEnumerable<(int X, int Y)> GetNeighbours(int[,] map, int x, int y)
{
    if (x > 0)
    {
        yield return (x - 1, y);
    }

    if (x < map.GetLength(0) - 1)
    {
        yield return (x + 1, y);
    }

    if (y > 0)
    {
        yield return (x, y - 1);
    }

    if (y < map.GetLength(1) - 1)
    {
        yield return (x, y + 1);
    }
}

void PrintGrid(int[,] grid)
{
    for (int i = 0; i < grid.GetLength(0); i++)
    {
        for (int j = 0; j < grid.GetLength(1); j++)
        {
            Console.Write(grid[i, j]);
        }
        Console.WriteLine();
    }
}
