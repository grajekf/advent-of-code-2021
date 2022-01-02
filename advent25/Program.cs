var inputLines = File.ReadAllLines("input.txt");

char[,] seaFloor = new char[inputLines.Length,inputLines.First().Length];

for (int y = 0; y < seaFloor.GetLength(0); y++)
{
    for (int x = 0; x < seaFloor.GetLength(1); x++)
    {
        seaFloor[y, x] = inputLines[y][x];
    }
}

bool change = true;
int step = 0;


while(change)
{
    step++;
    change = false;

    var toMoveEast = new List<(int X, int Y)>();

    for (int y = 0; y < seaFloor.GetLength(0); y++)
    {
        for (int x = 0; x < seaFloor.GetLength(1); x++)
        {
            if(seaFloor[y, x] != '>')
            {
                continue;
            }
            var nextEast = GetNextEast((x, y), seaFloor);
            if(seaFloor[nextEast.Y, nextEast.X] == '.')
            {
                toMoveEast.Add((x, y));
            }
        }
    }

    foreach (var toMove in toMoveEast)
    {
        change = true;
        seaFloor[toMove.Y, toMove.X] = '.';
        var nextEast = GetNextEast((toMove.X, toMove.Y), seaFloor);
        seaFloor[nextEast.Y, nextEast.X] = '>';
    }

    var toMoveSouth = new List<(int X, int Y)>();

    for (int y = 0; y < seaFloor.GetLength(0); y++)
    {
        for (int x = 0; x < seaFloor.GetLength(1); x++)
        {
            if (seaFloor[y, x] != 'v')
            {
                continue;
            }
            var nextSouth = GetNextSouth((x, y), seaFloor);
            if (seaFloor[nextSouth.Y, nextSouth.X] == '.')
            {
                toMoveSouth.Add((x, y));
            }
        }
    }

    foreach (var toMove in toMoveSouth)
    {
        change = true;
        seaFloor[toMove.Y, toMove.X] = '.';
        var nextSouth = GetNextSouth((toMove.X, toMove.Y), seaFloor);
        seaFloor[nextSouth.Y, nextSouth.X] = 'v';
    }
}

Console.WriteLine(step);

(int X, int Y) GetNextEast((int X, int Y) position, char[,] map)
{
    return ((position.X + 1) % map.GetLength(1), position.Y);
}

(int X, int Y) GetNextSouth((int X, int Y) position, char[,] map)
{
    return (position.X, (position.Y + 1) % map.GetLength(0));
}