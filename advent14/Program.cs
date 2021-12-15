using System.Text;

var lines = File.ReadAllLines("inputa.txt");

var polymer = lines[0];
var rules = lines.Skip(2).Select(l =>
{
    var parts = l.Split(" -> ");
    return (From: parts[0], To: parts[1]);
}).ToList();

int stepCount = 10;

for (int step = 1; step <= stepCount; step++)
{
    polymer = ApplyStep(polymer, rules);
}

Console.WriteLine(Score(polymer));

long Score(string polymer)
{
    var charChount = polymer.GroupBy(c => c).ToDictionary(c => c.Key, c => c.LongCount());
    return charChount.Max(c => c.Value) - charChount.Min(c => c.Value);
}

string ApplyStep(string polymer, IEnumerable<(string From, string To)> rules)
{
    var insertionArray = new string[polymer.Length + 1];

    foreach (var rule in rules)
    {
        foreach (var index in polymer.AllIndicesOf(rule.From))
        {
            insertionArray[index + 1] = rule.To;
        }
    }

    var sb = new StringBuilder();

    for (int i = 0; i < polymer.Length; i++)
    {
        var stringToInsert = insertionArray[i];
        if(!string.IsNullOrEmpty(stringToInsert))
        {
            sb.Append(stringToInsert);
        }
        sb.Append(polymer[i]);
    }

    return sb.ToString();
}


//KNP, https://stackoverflow.com/questions/2641326/finding-all-positions-of-substring-in-a-larger-string-in-c-sharp
static class StringExtensions
{
    public static IEnumerable<int> AllIndicesOf(this string text, string pattern)
    {
        if (string.IsNullOrEmpty(pattern))
        {
            throw new ArgumentNullException(nameof(pattern));
        }
        return Kmp(text, pattern);
    }

    private static IEnumerable<int> Kmp(string text, string pattern)
    {
        int M = pattern.Length;
        int N = text.Length;

        int[] lps = LongestPrefixSuffix(pattern);
        int i = 0, j = 0;

        while (i < N)
        {
            if (pattern[j] == text[i])
            {
                j++;
                i++;
            }
            if (j == M)
            {
                yield return i - j;
                j = lps[j - 1];
            }

            else if (i < N && pattern[j] != text[i])
            {
                if (j != 0)
                {
                    j = lps[j - 1];
                }
                else
                {
                    i++;
                }
            }
        }
    }

    private static int[] LongestPrefixSuffix(string pattern)
    {
        int[] lps = new int[pattern.Length];
        int length = 0;
        int i = 1;

        while (i < pattern.Length)
        {
            if (pattern[i] == pattern[length])
            {
                length++;
                lps[i] = length;
                i++;
            }
            else
            {
                if (length != 0)
                {
                    length = lps[length - 1];
                }
                else
                {
                    lps[i] = length;
                    i++;
                }
            }
        }
        return lps;
    }
}