var input = File.ReadAllLines("inputa.txt");

var columnCount = input.Length;
var rowCount = input.First().Length;

var heightmap = new int[columnCount, rowCount];

for(int i = 0; i < columnCount; i++)
{
    for(int j = 0; j < rowCount; j++)
    {
        heightmap[i, j] = int.Parse(input[i][j].ToString());
    }    
}

//a
var lowPoints = GetLowPoints(heightmap).ToList();
var riskSum = lowPoints.Select(p => heightmap[p.X, p.Y] + 1).Sum();

Console.WriteLine(riskSum);

//b
var basinSizes = GetBasinSizes(heightmap, lowPoints).OrderByDescending(b => b).Take(3);
Console.WriteLine(basinSizes.Aggregate(1, (r, b) => r* b));


IEnumerable<int> GetBasinSizes(int[,] map, IEnumerable<(int X, int Y)> lowPoints)
{
    foreach(var lowPoint in lowPoints)
    {
        HashSet<(int X, int Y)> visitedPoints = new HashSet<(int X, int Y)>();
        Queue<(int X, int Y)> queue = new Queue<(int X, int Y)>();

        queue.Enqueue(lowPoint);

        while(queue.Any())
        {
            var point = queue.Dequeue();
            visitedPoints.Add(point);

            var neighbours = GetNeighbours(map, point.X, point.Y);
            foreach(var neighbour in neighbours)
            {
                if(!visitedPoints.Contains(neighbour) && map[neighbour.X, neighbour.Y] != 9)
                {
                    queue.Enqueue(neighbour);
                }
            }
        }

        yield return visitedPoints.Count;
    }
}


IEnumerable<(int X, int Y)> GetLowPoints(int[,] map)
{
    for (int i = 0; i < map.GetLength(0); i++)
    {
        for (int j = 0; j < map.GetLength(1); j++)
        {
            var value = map[i, j];
            var neighbours = GetNeighbourHeights(map, i, j);
            if (value < neighbours.Min())
            {
                yield return (i, j);
            }
        }
    }
}

IEnumerable<int> GetNeighbourHeights(int[,] map, int x, int y)
{
    foreach (var neighbour in GetNeighbours(map, x, y))
    {
        yield return map[neighbour.X, neighbour.Y];
    }
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

