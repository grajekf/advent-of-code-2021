var lines = File.ReadAllLines("inputb.txt");

var polymer = lines[0];
var rules = lines.Skip(2).Select(l =>
{
    var parts = l.Split(" -> ");
    return (From: parts[0], To: parts[1]);
}).ToList();

var polymerTwogramCount = polymer.Zip(polymer.Skip(1)).GroupBy(t => t).ToDictionary(t => t.Key, t => t.LongCount());
var charCounts = polymer.GroupBy(t => t).ToDictionary(t => t.Key, t => t.LongCount());

int stepCount = 40;

for (int step = 1; step <= stepCount; step++)
{
    (polymerTwogramCount, charCounts) = ApplyStep(polymerTwogramCount, charCounts, rules);
}

Console.WriteLine(Score(charCounts));




(Dictionary<(char First, char Second), long>, Dictionary<char, long>) ApplyStep(Dictionary<(char First, char Second), long> polymer,
    Dictionary<char, long> charCounts,
    IEnumerable<(string From, string To)> rules)
{
    var result = new Dictionary<(char First, char Second), long>(polymer);
    var resultCounts = new Dictionary<char, long>(charCounts);

    foreach (var rule in rules)
    {
        var twoGramToFind = (First: rule.From[0], Second: rule.From[1]);
        if (polymer.ContainsKey(twoGramToFind) && polymer[twoGramToFind] > 0)
        {
            result[twoGramToFind] = 0L;
        }
    }

    foreach (var rule in rules)
    {
        var twoGramToFind = (First: rule.From[0], Second: rule.From[1]);
        var leftNewTwoGram = (First: rule.From[0], Second: rule.To[0]);
        var rightNewTwoGram = (First: rule.To[0], Second: rule.From[1]);

        if(polymer.ContainsKey(twoGramToFind) && polymer[twoGramToFind] > 0)
        {
            if (!result.ContainsKey(leftNewTwoGram))
            {
                result[leftNewTwoGram] = 0L;
            }
            if (!result.ContainsKey(rightNewTwoGram))
            {
                result[rightNewTwoGram] = 0L;
            }
            if(!resultCounts.ContainsKey(rule.To[0]))
            {
                resultCounts[rule.To[0]] = 0L;
            }

            result[leftNewTwoGram] += polymer[twoGramToFind];
            result[rightNewTwoGram] += polymer[twoGramToFind];
            resultCounts[rule.To[0]] += polymer[twoGramToFind];

        }
    }

    return (result, resultCounts);
}

long Score(Dictionary<char, long> charCounts)
{
    return charCounts.Max(c => c.Value) - charCounts.Min(c => c.Value);
}