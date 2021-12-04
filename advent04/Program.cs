// See https://aka.ms/new-console-template for more information
var inputa = File.ReadAllText("inputa.txt");
var parts = inputa.Split("\r\n\r\n", StringSplitOptions.RemoveEmptyEntries);
var bingoNumbers = parts[0].Split(",", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
var boards = parts.Skip(1).Select(s => BingoBoard.Parse(s)).ToList();

//a
foreach (var number in bingoNumbers)
{
    foreach (var board in boards)
    {
        board.Cross(number);
    }

    var winningBoard = boards.Where(b => b.IsWinning()).FirstOrDefault();
    if(winningBoard != null)
    {
        Console.WriteLine(winningBoard.GetWinningValue(number));
        break;
    }
}

//b
foreach (var board in boards)
{
    board.Reset();
}

foreach (var number in bingoNumbers)
{
    foreach (var board in boards)
    {
        board.Cross(number);
    }

    BingoBoard lastBoard = boards.First();

    boards = boards.Where(b => !b.IsWinning()).ToList();

    if(!boards.Any())
    {
        Console.WriteLine(lastBoard.GetWinningValue(number));
        break;
    }
}




class BingoBoard
{
    private BingoField[,] board;

    protected BingoBoard(BingoField[,] board)
    {
        this.board = board;
    }

    public static BingoBoard Parse(string input)
    {
        var lines = input.Split("\r\n").Where(l => !string.IsNullOrEmpty(l)).ToList();
        var board = new BingoField[lines.Count, lines.Count];
        for(int i = 0; i < lines.Count; i++)
        {
            var line = lines[i];
            var numbers = line.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
            for(int j = 0; j < numbers.Count; j++)
            {
                board[i, j] = new BingoField(numbers[j], false);
            }
        }

        return new BingoBoard(board);
    }

    public bool IsWinning()
    {
        return GetAllRows().Any(r => r.All(f => f.Crossed))
            || GetAllColumns().Any(c => c.All(f => f.Crossed));
    }

    public void Cross(int value)
    {
        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                board[i, j].Cross(value);
            }
        }
    }

    public void Reset()
    {
        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                board[i, j].Reset();
            }
        }
    }

    public int GetWinningValue(int lastNumber)
    {
        var unmarkedSum = GetUnmarked().Sum(u => u.Value);
        return unmarkedSum * lastNumber;
    }

    private IEnumerable<BingoField> GetUnmarked()
    {
        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                if(!board[i, j].Crossed)
                {
                    yield return board[i, j];
                }
            }
        }
    }

    private IEnumerable<BingoField> GetRow(int row)
    {
        for(int i = 0; i < board.GetLength(1); i++)
        {
            yield return board[row, i];
        }
    }

    private IEnumerable<IEnumerable<BingoField>> GetAllRows()
    {
        for(int i = 0;i < board.GetLength(0); i++)
        {
            yield return GetRow(i);
        }
    }

    private IEnumerable<BingoField> GetColumn(int column)
    {
        for (int i = 0; i < board.GetLength(0); i++)
        {
            yield return board[i, column];
        }
    }

    private IEnumerable<IEnumerable<BingoField>> GetAllColumns()
    {
        for (int i = 0; i < board.GetLength(1); i++)
        {
            yield return GetColumn(i);
        }
    }

}

class BingoField
{
    public BingoField()
    {

    }
    public BingoField(int value, bool crossed)
    {
        Value = value;
        Crossed = crossed;
    }

    public bool Cross(int value)
    {
        if(!Crossed && Value == value)
        {
            Crossed = true;
            return true;
        }

        return false;
    }

    public void Reset()
    {
        Crossed = false;
    }

    public int Value { get; private set; }
    public bool Crossed { get; private set; }
}
