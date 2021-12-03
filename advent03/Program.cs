var binaries = File.ReadAllLines("inputa.txt");

var length = binaries.First().Length;
int[][] counts = new int[length][];
for(int i = 0; i < length; i++)
{
    counts[i] = new int[2];
}

const int ZeroAsciiCode = 48;

foreach (var binary in binaries)
{
    for(int i = 0; i < length; i++)
    {
        counts[i][binary[i] - ZeroAsciiCode]++;
    }
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


