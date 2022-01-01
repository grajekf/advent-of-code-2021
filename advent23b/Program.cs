﻿// See https://aka.ms/new-console-template for more information
var board = Board.Parse(File.ReadAllText("input.txt"));
//var nextStates = board.GetNextStates().ToList();
//Console.WriteLine(board.Cost);

var minCost = long.MaxValue;

var boardStack = new Stack<Board>();

boardStack.Push(board);

while(boardStack.Count > 0)
{
    var current = boardStack.Pop();

    if(current.ProjectedMinCost() > minCost)
    {
        continue;
    }

    if(current.IsFinished())
    {
        var cost = current.Cost;

        if(cost < minCost)
        {
            minCost = cost;
            Console.WriteLine(minCost);
        }
    }
    else
    {
        var nextStates = current.GetNextStates().OrderByDescending(s => s.Cost).ToList();

        foreach (var state in nextStates)
        {
            boardStack.Push(state);
        }
    }
}

Console.WriteLine();
Console.WriteLine(minCost);
Console.ReadKey();


class Board
{

    public long Cost { get; set; }

    public Board(long cost, bool[,] mask, params (int X, int Y)[] positions)
    {
        _positions = positions.ToArray();
        Cost = cost;
        _boardMask = (bool[,])mask.Clone();
        _wallMask = (bool[,])mask.Clone();
        _groups = new int[mask.GetLength(0), mask.GetLength(1)];
        var index = 0;
        foreach (var position in _positions)
        {
            _boardMask[position.Y, position.X] = true;
            var group = index / 4;
            _groups[position.Y, position.X] = group;
            index++;
        }

        _distances = new int[_positions.Length][,];
        for (int i = 0; i < _positions.Length; i++)
        {
            _distances[i] = new int[_boardMask.GetLength(0), _boardMask.GetLength(1)];

            var queue = new Queue<(int X, int Y)>();
            queue.Enqueue(_positions[i]);

            while (queue.Any())
            {
                var current = queue.Dequeue();
                var currentValue = _distances[i][current.Y, current.X];
                var neighbours = GetNeighbours(_distances[i], current.X, current.Y).ToList();

                foreach (var neighbour in neighbours)
                {
                    if (_distances[i][neighbour.Y, neighbour.X] > 0)
                    {
                        continue;
                    }

                    if (_boardMask[neighbour.Y, neighbour.X])
                    {
                        continue;
                    }

                    _distances[i][neighbour.Y, neighbour.X] = currentValue + 1;

                    queue.Enqueue(neighbour);
                }
            }
        }
    }

    public bool IsFinished()
    {
        for (int i = 0; i < _positions.Length; i++)
        {
            if(!IsInRightPlace(i))
            {
                return false;
            }
        }

        return true;
    }


    private (int X, int Y)[] _positions;
    private bool[,] _boardMask;
    private bool[,] _wallMask;
    private int[][,] _distances;
    private int[,] _groups;

    private static int[] _moveCosts = new int[] { 1, 1, 1, 1, 10, 10, 10, 10, 100, 100, 100, 100, 1000, 1000, 1000, 1000 };
    private static int[] _forbiddenHallX = new int[] { 3, 5, 7, 9 };

    private static bool IsInHall((int X, int Y) position)
    {
        return position.Y == 1;
    }

    public static Board Parse(string input)
    {
        var lines = input.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
        var rowCount = lines.Length;
        var colCount = lines.First().Length;

        var mask = new bool[rowCount, colCount];
        var aList = new List<(int X, int Y)>();
        var bList = new List<(int X, int Y)>();
        var cList = new List<(int X, int Y)>();
        var dList = new List<(int X, int Y)>();

        for (int i = 0; i < rowCount; i++)
        {
            for (int j = 0; j < lines[i].Length; j++)
            {
                if (lines[i][j] == '#')
                {
                    mask[i, j] = true;
                }

                if (lines[i][j] == 'A')
                {
                    aList.Add((j, i));
                }
                if (lines[i][j] == 'B')
                {
                    bList.Add((j, i));
                }
                if (lines[i][j] == 'C')
                {
                    cList.Add((j, i));
                }
                if (lines[i][j] == 'D')
                {
                    dList.Add((j, i));
                }
            }
        }
        return new Board(0, mask, 
            aList[0], aList[1], aList[2], aList[3], 
            bList[0], bList[1], bList[2], bList[3],
            cList[0], cList[1], cList[2], cList[3],
            dList[0], dList[1], dList[2], dList[3]);
    }

    private static IEnumerable<(int X, int Y)> GetNeighbours(int[,] map, int x, int y)
    {
        if (x > 0)
        {
            yield return (x - 1, y);
        }

        if (x < map.GetLength(1) - 1)
        {
            yield return (x + 1, y);
        }

        if (y > 0)
        {
            yield return (x, y - 1);
        }

        if (y < map.GetLength(0) - 1)
        {
            yield return (x, y + 1);
        }
    }

    public IEnumerable<Board> GetNextStates()
    {
        for (int i = 0; i < _positions.Length; i++)
        {
            var inRightPlace = IsInRightPlace(i);
            var partnersInRightPlace = GetPartnerIndices(i).Select(i => IsInRightPlace(i));


            if(inRightPlace && partnersInRightPlace.All(x => x))
            {
                continue;
            }

            if(inRightPlace && UnderPositionAllRight(_positions[i], i / 4))
            {
                continue;
            }

            var rightX = GetRightX(i);

            for(int y = 2; y <= 5; y++)
            {
                if(_distances[i][y, rightX] > 0 && UnderPositionAllRight((rightX, y), i / 4))
                {
                    var newPositions = GetPositionsAfterMove(i, (rightX, y));
                    yield return new Board(Cost + _distances[i][y, rightX] * _moveCosts[i], _wallMask, newPositions);
                }
            }

            if (!IsInHall(_positions[i]))
            {
                for(int x = 0; x < _distances[i].GetLength(1); x++)
                {
                    if(_distances[i][1, x] == 0)
                    {
                        continue;
                    }

                    if(_forbiddenHallX.Contains(x))
                    {
                        continue;
                    }

                    var newPositions = GetPositionsAfterMove(i, (x, 1));
                    yield return new Board(Cost + _distances[i][1, x] * _moveCosts[i], _wallMask, newPositions);

                }
            }
        }
    }

    public long ProjectedMinCost()
    {
        var cost = Cost;
        for (int i = 0; i < _positions.Length; i++)
        {
            if(!IsInRightPlace(i))
            {
                var rightX = GetRightX(i);
                cost += (Math.Abs(_positions[i].X - rightX) + Math.Abs(_positions[i].Y - 1) + 1) * _moveCosts[i];
            }
        }

        return cost;
    }

    private (int X, int Y)[] GetPositionsAfterMove(int index, (int X, int Y) moveTo)
    {
        var newPositions = ((int X, int Y)[])_positions.Clone();
        newPositions[index] = moveTo;

        return newPositions;
    }

    private static int GetRightX(int index)
    {
        return (index / 4) * 2 + 3;
    }

    private static IEnumerable<int> GetPartnerIndices(int index)
    {
        var baseIndex = index / 4;

        for (int i = baseIndex; i < baseIndex + 4; i++)
        {
            if(index != i)
            {
                yield return i;
            }
        }
    }

    private bool IsInRightPlace(int index)
    {
        var position = _positions[index];

        if(IsInHall(position))
        {
            return false;
        }

        var correctX = GetRightX(index);

        return position.X == correctX;
    } 

    private bool UnderPositionAllRight((int X, int Y)position, int group)
    {
        for (int y = position.Y + 1; y <= 5; y++)
        {
            if(_groups[y, position.X] != group)
            {
                return false;
            }
        }

        return true;
    }
}

