var inputNumbers = File.ReadAllText("inputa.txt").Split(",").Select(int.Parse).ToList();

const int interval = 7;
const int newInterval = interval + 2;
const int generationCount = 80;
const int extendedGenerationCount = 256;

var timers = new long[newInterval];
foreach (var num in inputNumbers)
{
    timers[num]++;
}

Simulate(timers, generationCount);


Console.WriteLine(timers.Sum());

Simulate(timers, extendedGenerationCount - generationCount);


Console.WriteLine(timers.Sum());


void Simulate(long[] fishes, int generationCount)
{
    for (int i = 0; i < generationCount; i++)
    {
        long newFishCount = fishes[0];
        for (int j = 1; j < newInterval; j++)
        {
            fishes[j - 1] = fishes[j];
        }

        fishes[interval - 1] += newFishCount;
        fishes[newInterval - 1] = newFishCount;
    }
}