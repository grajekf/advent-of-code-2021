using System.Text;

var instructions = File.ReadAllLines("input.txt").Select(Instruction.Parse).ToList();

var rng = new Random();


int populationSize = 1000;
int genCount = 100;
int tournamentSize = 5;
var population = new List<string>();
var mutationRate = 0.1;

while (population.Count < populationSize)
{
    var i = RandomLong(rng, 11111111111111, 99999999999999);
    var iString = i.ToString();
    if (iString.Contains('0'))
    {
        continue;
    }

    StringBuilder sb = new StringBuilder(iString);
    sb[8] = '7';
    sb[9] = '1';
    sb[10] = '9';
    sb[11] = '1';
    sb[12] = '1';
    sb[1] = '9';
    sb[0] = '9';
    sb[2] = '9';
    sb[3] = '9';
    sb[4] = '5';
    iString = sb.ToString();

    population.Add(iString);
}

var acceptableMaxNumbers = GetAcceptableNumbers(population,
    instructions,
    (withFitness, randomSample) => withFitness[randomSample.MinBy(i => withFitness[i].Item2.GetValue('z') - (double)long.Parse(withFitness[i].Item1) / (double)99999999999999)].Item1,
    (newNumber) => newNumber.StartsWith("999959"),
    genCount,
    tournamentSize,
    mutationRate
);

Console.WriteLine($"Max {acceptableMaxNumbers.Max()}");


population = new List<string>();

while (population.Count < populationSize)
{
    var i = RandomLong(rng, 48111111111111, 48111558819211);
    var iString = i.ToString();
    if (iString.Contains('0'))
    {
        continue;
    }

    StringBuilder sb = new StringBuilder(iString);
    iString = sb.ToString();

    population.Add(iString);
}

var acceptableMinNumbers = GetAcceptableNumbers(population,
    instructions,
    (withFitness, randomSample) => withFitness[randomSample.MinBy(i => withFitness[i].Item2.GetValue('z')/* + (double)long.Parse(withFitness[i].Item1) / (double)99999999999999*/)].Item1,
    (newNumber) => newNumber.StartsWith("48111") && newNumber[2] == newNumber[3],
    genCount,
    tournamentSize,
    mutationRate
);

Console.WriteLine($"Min {acceptableMinNumbers.Min()}");


IEnumerable<long> GetAcceptableNumbers(IEnumerable<string> initialPopulation,
    IEnumerable<Instruction> instructions,
    Func<IList<(string, State)>, IEnumerable<int>, string> parentSelector,
    Func<string, bool> newNumberPredicate,
    int genCount,
    int tournamentSize, 
    double mutationRate)
{

    var rng = new Random();

    var population = initialPopulation.ToList();
    for (int gen = 1; gen <= genCount; gen++)
    {
        var withFitness = population.Zip(population.Select(p => Run(instructions, p.Select(t => long.Parse(t.ToString())).ToList())).ToList()).OrderBy(x => x.Second.GetValue('z')).ToList();
        var newPopulation = new HashSet<string>();

        while (newPopulation.Count < populationSize)
        {
            var randomSampleA = Enumerable.Range(0, withFitness.Count).OrderBy(x => Guid.NewGuid()).Take(tournamentSize).ToList();
            var randomSampleB = Enumerable.Range(0, withFitness.Count).OrderBy(x => Guid.NewGuid()).Take(tournamentSize).ToList();

            var parentA = parentSelector(withFitness, randomSampleA);
            var parentB = parentSelector(withFitness, randomSampleA);

            var newNumber = Mutate(rng, mutationRate, CrossOver(rng, parentA, parentB));

            if (!newNumberPredicate(newNumber))
            {
                continue;
            }

            newPopulation.Add(newNumber);
        }

        var accepted = withFitness.Where(x => x.Second.GetValue('z') == 0).ToList();
        foreach (var n in accepted)
        {
            yield return long.Parse(n.First);
        }

        population = newPopulation.ToList();

        Console.WriteLine($"Gen. {gen}, {withFitness.First().First}, {withFitness.First().Second}");
    }
}


string CrossOver(Random random, string a, string b)
{
    var point = random.Next(0, a.Length - 1);

    return a[..point] + b[point..];
}

string Mutate(Random random, double rate, string a)
{
    var sb = new StringBuilder(a);

    for (int i = 0; i < a.Length; i++)
    {
        if(random.NextDouble() < rate)
        {
            sb[i] = random.Next(1, 10).ToString()[0];
        }
    }


    return sb.ToString();
}




long RandomLong(Random rng, long min, long max)
{
    return min + (long)(rng.NextDouble() * (max - min));
}


State Run(IEnumerable<Instruction> instructions, IEnumerable<long> input)
{
    var state = new State();

    foreach (var instruction in instructions)
    {
        (state, input) = state.Next(instruction, input);
    }
    return state;
}


class State
{
    public State()
    {
        Variables = new long[4];
    }
    public State(long[] variables)
    {
        Variables = variables.ToArray();
    }

    public long[] Variables { get; private set; }


    public (State NextState, IEnumerable<long> RemainingInput) Next(Instruction instruction, IEnumerable<long> input)
    {
        var variables = Variables.ToArray();
        switch(instruction.Text)
        {
            case "inp":
                variables[VariableToIndex(instruction.Param1)] = input.First();
                return (new State(variables), input.Skip(1).ToList());
            case "add":
                variables[VariableToIndex(instruction.Param1)] = 
                    variables[VariableToIndex(instruction.Param1)] + (instruction.IntParam2 ?? variables[VariableToIndex(instruction.CharParam2.Value)]);
                return (new State(variables), input);
            case "mul":
                variables[VariableToIndex(instruction.Param1)] =
                    variables[VariableToIndex(instruction.Param1)] * (instruction.IntParam2 ?? variables[VariableToIndex(instruction.CharParam2.Value)]);
                return (new State(variables), input);
            case "div":
                variables[VariableToIndex(instruction.Param1)] =
                    variables[VariableToIndex(instruction.Param1)] / (instruction.IntParam2 ?? variables[VariableToIndex(instruction.CharParam2.Value)]);
                return (new State(variables), input);
            case "mod":
                variables[VariableToIndex(instruction.Param1)] =
                    variables[VariableToIndex(instruction.Param1)] % (instruction.IntParam2 ?? variables[VariableToIndex(instruction.CharParam2.Value)]);
                return (new State(variables), input);
            case "eql":
                variables[VariableToIndex(instruction.Param1)] =
                    Convert.ToInt32(variables[VariableToIndex(instruction.Param1)] == (instruction.IntParam2 ?? variables[VariableToIndex(instruction.CharParam2.Value)]));
                return (new State(variables), input);
            default:
                throw new Exception("Unrecognized instruction");
        }

    }

    private int VariableToIndex(char variable)
    {
        return variable - 119;
    }

    public long GetValue(char variable)
    {
        return Variables[VariableToIndex(variable)];
    }

    public override string ToString()
    {
        return $"x: {GetValue('x')}, y: {GetValue('y')}, z: {GetValue('z')}, w: {GetValue('w')}";
    }
}

class Instruction
{
    public Instruction(string text, char param1)
    {
        Text = text;
        Param1 = param1;
    }

    public Instruction(string text, char param1, char charParam2)
    {
        Text = text;
        Param1 = param1;
        CharParam2 = charParam2;
    }

    public Instruction(string text, char param1, int intParam2)
    {
        Text = text;
        Param1 = param1;
        IntParam2 = intParam2;
    }

    public string Text { get; set; }
    public char Param1 { get; set; }
    public char? CharParam2 { get; set; }
    public int? IntParam2 { get; set; }

    public static Instruction Parse(string input)
    {
        var parts = input.Split(" ", StringSplitOptions.RemoveEmptyEntries);
        var text = parts[0];
        var param1 = parts[1][0];

        if(parts.Length > 2)
        {
            var param2 = parts[2];
            if(int.TryParse(param2, out var intParam2))
            {
                return new Instruction(text, param1, intParam2);
            }
            else
            {
                return new Instruction(text, param1, param2[0]);
            }
        }

        return new Instruction(text, param1);
    }

    public override string ToString()
    {
        return $"{Text} {Param1} {IntParam2}{CharParam2}";
    }
}