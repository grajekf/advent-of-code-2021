var binaries = File.ReadAllLines("inputa.txt");
const int ZeroAsciiCode = 48;
//a
var length = binaries.First().Length;
int[][] counts = new int[length][];
for (int i = 0; i < length; i++)
{
    counts[i] = GetCountsForBit(binaries, i);
}


var gamma = 0L;
var epsilon = 0L;
var multiplier = 1L;

for (int i = length - 1; i >= 0; i--)
{
    //Console.WriteLine($"[0]: {counts[i][0]}, [1]: {counts[i][1]}");
    var maxIndex = counts[i].ToList().IndexOf(counts[i].Max());
    var minIndex = counts[i].ToList().IndexOf(counts[i].Min());

    gamma += multiplier * maxIndex;
    epsilon += multiplier * minIndex;

    multiplier <<= 1;
}

Console.WriteLine(Convert.ToString(gamma, 2).PadLeft(length));
Console.WriteLine(Convert.ToString(epsilon, 2).PadLeft(length));
Console.WriteLine(gamma*epsilon);


//b

var oxygenCandidates = binaries.ToList();
var oxygenString = GetRating(oxygenCandidates, OxygenCriterium);
var oxygenRating = Convert.ToInt64(oxygenString, 2);

var co2Candidates = binaries.ToList();
var co2String = GetRating(co2Candidates, CO2Criterium);
var co2Rating = Convert.ToInt64(co2String, 2);

Console.WriteLine(oxygenString);
Console.WriteLine(co2String);
Console.WriteLine(oxygenRating * co2Rating);

bool OxygenCriterium(char bit, int[] curCounts)
{
    var maxIndex = 1;
    if(curCounts[0] > curCounts[maxIndex])
    {
        maxIndex = 0;
    }
    return (bit - ZeroAsciiCode) == maxIndex;
}

bool CO2Criterium(char bit, int[] curCounts)
{
    var minIndex = 0;
    if (curCounts[1] < curCounts[minIndex])
    {
        minIndex = 1;
    }
    return (bit - ZeroAsciiCode) == minIndex;
}

string GetRating(IEnumerable<string> candidates, Func<char, int[], bool> bitCriterium)
{
    for(int i = 0; i < counts.Length; i++)
    {
        if(candidates.Count() == 1)
        {
            return candidates.Single();
        }

        candidates = candidates.Where(c => bitCriterium(c[i], GetCountsForBit(candidates, i))).ToList();

        //Console.WriteLine(string.Join(", ", candidates));
    }

    return candidates.Single();
}

int[] GetCountsForBit(IEnumerable<string> candidates, int i)
{
    var result = new int[2];
    foreach (var candidate in candidates)
    {
        result[candidate[i] - ZeroAsciiCode]++;
    }

    return result;
}

