using System.Collections;

var lines = File.ReadAllLines("input.txt").Where(l => !string.IsNullOrEmpty(l));

var algorithm = lines.First();

var imagelines = lines.Skip(1).ToList();
var specialPixels = new HashSet<(int X, int Y)>();

for (int y = 0; y < imagelines.Count; y++)
{
    for (int x = 0; x < imagelines[y].Length; x++)
    {
        if(imagelines[y][x] == '#')
        {
            specialPixels.Add((x, y));
        }
    }
}

bool isSpecialLit = true;
for(int step = 1; step <= 50; step++)
{
    specialPixels = Step(specialPixels, algorithm, isSpecialLit);
    isSpecialLit = !isSpecialLit;

    if(step == 2)
    {
        //a
        Console.WriteLine(specialPixels.Count);
    }
}

//b
Console.WriteLine(specialPixels.Count);






HashSet<(int X, int Y)> Step(HashSet<(int X, int Y)> image, string algorithm, bool isSpecialLit)
{
    HashSet<(int X, int Y)> result = new HashSet<(int X, int Y)>();

    var allRelevant = image.SelectMany(p => GetSquare(p.X, p.Y, 3)).ToHashSet();

    foreach (var relevant in allRelevant)
    {
        var relevantBinary = new BitArray(GetSquare(relevant.X, relevant.Y, 3).Select(p => image.Contains(p) == isSpecialLit).Reverse().ToArray());
        //var relevantBinary = new BitArray(GetSquare(relevant.X, relevant.Y, 3).Select(p => image.Contains(p)).Reverse().ToArray());
        var index = GetIntFromBitArray(relevantBinary);

        var newValue = algorithm[index] == '#';
        if (newValue ^ isSpecialLit)
        //if (newValue)
        {
            result.Add(relevant);
        }
    }
    return result;
}

IEnumerable<(int X, int Y)> GetSquare(int x, int y, int size)
{
    for (int i = -size/2; i <= size/2; i++)
    {
        for (int j = -size / 2; j <= size / 2; j++)
        {
            yield return (x + j, y + i);
        }
    }
}

int GetIntFromBitArray(BitArray bitArray)
{
    int[] array = new int[1];
    bitArray.CopyTo(array, 0);
    return array[0];
}