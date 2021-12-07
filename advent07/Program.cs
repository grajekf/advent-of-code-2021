var positions= File.ReadAllText("inputa.txt").Split(",").Select(int.Parse).ToArray();


//a
Console.WriteLine(FindBestPosition(positions, CalculateFuel));
//b
Console.WriteLine(FindBestPosition(positions, CalculateFuelB));



int CalculateFuel(IEnumerable<int> positions, int newPosition)
{
    return positions.Select(x => Math.Abs(x - newPosition)).Sum();
}

int CalculateFuelB(IEnumerable<int> positions, int newPosition)
{
    return positions.Select(x => (Math.Abs(x - newPosition) * (Math.Abs(x - newPosition ) + 1)) / 2).Sum();
}

int FindBestPosition(IEnumerable<int> positions, Func<IEnumerable<int>, int, int> costFunc)
{
    var a = positions.Min();
    var b = positions.Max();

    while (Math.Abs(a - b) > 1)
    {
        var fuelA = costFunc(positions, a);
        var fuelB = costFunc(positions, b);

        var pivot = (a + b) / 2;
        if (fuelA < fuelB)
        {
            b = pivot;
        }
        else
        {
            a = pivot;
        }
    }

    return Math.Min(costFunc(positions, a), costFunc(positions, b));
}