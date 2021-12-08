var numberDict = new Dictionary<string, int>()
{
    {"abcefg", 0},
    {"cf", 1},
    {"acdeg", 2},
    {"acdfg", 3},
    {"bcdf", 4},
    {"abdfg", 5},
    {"abdefg", 6},
    {"acf", 7},
    {"abcdefg", 8},
    {"abcdfg", 9},
};

var numbers = new string[] { "abcefg", "cf", "acdeg", "acdfg", "bcdf", "abdfg", "abdefg", "acf", "abcdefg", "abcdfg" };

var signalLines = File.ReadAllLines("inputa.txt").Select(l => SignalLine.Parse(l, numberDict, numbers)).ToList();

//a
var oneCount = signalLines.Sum(s => s.GetNumbersWithLength(2).Count());
var fourCount = signalLines.Sum(s => s.GetNumbersWithLength(4).Count());
var sevenCount = signalLines.Sum(s => s.GetNumbersWithLength(3).Count());
var eightCount = signalLines.Sum(s => s.GetNumbersWithLength(7).Count());

Console.WriteLine(oneCount + fourCount + sevenCount + eightCount);

//b
Console.WriteLine(signalLines.Sum(l => l.GetOutput()));


class SignalLine
{
    public SignalLine(IEnumerable<string> signalPatters,
        IEnumerable<string> digitOutputs,
        IDictionary<string, int> numberDict, 
        string[] numbers)
    {
        SignalPatters = signalPatters.ToList();
        DigitOutputs = digitOutputs.ToList();
        ScrambledNumbers = new string[numbers.Length];
        NumberDict = numberDict;
        Numbers = numbers;
    }

    public IEnumerable<string> SignalPatters { get; set; }
    public IEnumerable<string> DigitOutputs { get; set; }
    public IDictionary<string, int> NumberDict { get; set; }
    public string[] Numbers { get; set; }
    public string[] ScrambledNumbers { get; set; }

    public IEnumerable<string> GetNumbersWithLength(int length)
    {
        return DigitOutputs.Where(d => d.Length == length).ToList();
    }

    public IEnumerable<string> GetSignalsWithLength(int length)
    {
        return SignalPatters.Where(d => d.Length == length).ToList();
    }

    public int GetOutput()
    {
        var wireMap = GenerateWireMap();

        var result = string.Empty;

        foreach(var digit in DigitOutputs)
        {
            result += wireMap[digit];
        }

        return int.Parse(result);
    }

    private IDictionary<string, int> GenerateWireMap()
    {
        var possibleMappings = new List<(char From, char To)>();

        ScrambledNumbers[1] = GetSignalsWithLength(2).Single();
        ScrambledNumbers[7] = GetSignalsWithLength(3).Single();

        var a = ScrambledNumbers[7].Where(s => !ScrambledNumbers[1].Contains(s)).Single();

        ScrambledNumbers[4] = GetSignalsWithLength(4).Single();

        var d = GetSignalsWithLength(5).IntersectAll().Intersect(ScrambledNumbers[4]).Single();

        ScrambledNumbers[0] = GetSignalsWithLength(6).Where(s => !s.Contains(d)).Single();

        var b = ScrambledNumbers[0].Intersect(ScrambledNumbers[4]).Where(x => !ScrambledNumbers[1].Contains(x)).Single();

        ScrambledNumbers[5] = GetSignalsWithLength(5).Where(s => s.Contains(b)).Single();
        ScrambledNumbers[6] = GetSignalsWithLength(6).Where(s => !ScrambledNumbers[1].All(x => s.Contains(x))).Single();

        var e = ScrambledNumbers[6].Where(x => !ScrambledNumbers[5].Contains(x)).Single();

        var c = "abcdefg".Where(x => !ScrambledNumbers[6].Contains(x)).Single();
        var f = ScrambledNumbers[1].Where(x => x != c).Single();

        var g = "abcdefg".Where(x => !ScrambledNumbers[4].Contains(x) && x != a && x != e).Single();

        ScrambledNumbers[2] = string.Concat(string.Concat(a, c, d, e, g).OrderBy(x => x));
        ScrambledNumbers[3] = string.Concat(string.Concat(a, c, d, f, g).OrderBy(x => x));
        ScrambledNumbers[8] = GetSignalsWithLength(7).Single();
        ScrambledNumbers[9] = string.Concat(string.Concat(a, b, c, d, f, g).OrderBy(x => x));

        return Enumerable.Range(0, Numbers.Length).ToDictionary(i => ScrambledNumbers[i], i => i );
    }



    public static SignalLine Parse(string input, IDictionary<string, int> numberDict, string[] numbers)
    {
        var parts = input.Split(" | ");
        return new SignalLine(parts[0].Split(" ").Select(ToOrderedString), parts[1].Split(" ").Select(ToOrderedString), numberDict, numbers);
    }

    private static string ToOrderedString(string input)
    {
        return String.Concat(input.OrderBy(c => c));
    }
}

public static class StringIEnumerableExtensions
{
    public static string IntersectAll(this IEnumerable<string> strings)
    {
        if (!strings.Any())
            return string.Empty;

        var result = strings.First();

        return strings.Skip(1).Aggregate(result, (r, s) => string.Concat(r.Intersect(s)));
    }
}