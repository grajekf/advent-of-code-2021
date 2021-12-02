

var lines = File.ReadAllLines("inputa.txt");
var moves = lines.Select(line => Move.Parse(line));
var startPosition = (X: 0, Y: 0);
var endPosition = moves.Aggregate(startPosition, (p, m) => m.Execute(p));

Console.WriteLine(endPosition.X * endPosition.Y);




class Move
{
    public Move(string direction, int moveBy)
    {
        Direction = direction;
        MoveBy = moveBy;
    }

    public static Move Parse(string input)
    {
        var parts = input.Split(' ');
        return new Move(parts[0], int.Parse(parts[1]));
    }

    public string Direction { get; set; }
    public int MoveBy { get; set; }

    public (int X, int Y) Execute((int X, int Y) position)
    {
        int x = position.X;
        int y = position.Y;

        switch(Direction)
        {
            case "forward":
                x += MoveBy;
                break;
            case "up":
                y -= MoveBy;
                break;
            case "down":
                y += MoveBy;
                break;
        }

        return (x, y);
    }
}

