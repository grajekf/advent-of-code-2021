

var lines = File.ReadAllLines("inputb.txt");
var moves = lines.Select(line => Move.Parse(line));
var startPosition = (X: 0l, Depth: 0l, Aim: 0l);
var endPosition = moves.Aggregate(startPosition, (p, m) => m.Execute(p));

Console.WriteLine(endPosition.X * endPosition.Depth);




class Move
{
    public Move(string direction, long moveBy)
    {
        Direction = direction;
        MoveBy = moveBy;
    }

    public static Move Parse(string input)
    {
        var parts = input.Split(' ');
        return new Move(parts[0], long.Parse(parts[1]));
    }

    public string Direction { get; set; }
    public long MoveBy { get; set; }

    public (long X, long Depth, long Aim) Execute((long X, long Depth, long Aim) position)
    {
        long x = position.X;
        long depth = position.Depth;
        long aim = position.Aim;

        switch(Direction)
        {
            case "forward":
                x += MoveBy;
                depth += aim * MoveBy;
                break;
            case "up":
                aim -= MoveBy;
                break;
            case "down":
                aim += MoveBy;
                break;
        }

        return (x, depth, aim);
    }
}

