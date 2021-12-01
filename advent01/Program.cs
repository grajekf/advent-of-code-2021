// See https://aka.ms/new-console-template for more information
var depths = ReadInput("input_a.txt").ToList();

//a
Console.WriteLine(CountIncreasing(depths));

//b
var tripltes = NLets(depths, 3);
var tripletSums = tripltes.Select(tripltes => tripltes.Sum());
Console.WriteLine(CountIncreasing(tripletSums));


static IEnumerable<int> ReadInput(string fileName)
{
    return File.ReadAllLines(fileName).Select(int.Parse).ToList();
}

static int CountIncreasing(IEnumerable<int> seq)
{
    return seq.Zip(seq.Skip(1)).Count(p => p.Second > p.First);
}

static IEnumerable<IEnumerable<T>> NLets<T>(IList<T> seq, int n)
{
    for(int i = 0; i < seq.Count; i++)
    {
        yield return seq.Skip(i).Take(n);
    }
}