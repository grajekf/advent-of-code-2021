using System.Linq;

var scanningResults = File.ReadAllText("input.txt").Split("\r\n\r\n").Select(s =>
{
    var lines = s.Split("\r\n", StringSplitOptions.RemoveEmptyEntries).Where(l => !l.Contains("scanner"));
    return new ScanningResult(lines.Select(l =>
    {
        var parts = l.Split(",").Select(int.Parse).ToList();

        return new Position(parts[0], parts[1], parts[2]);
    }).ToHashSet());
}
).ToList();

var fixedScanningResults = new List<ScanningResult>();
var first = scanningResults.First();
fixedScanningResults.Add(first);
var allPoints = new HashSet<Position>(first.Positions);
scanningResults = scanningResults.Skip(1).ToList();

while (scanningResults.Any())
{
    bool change = false;
    foreach (var scanningResult in scanningResults)
    {
        foreach (var reference in fixedScanningResults)
        {
            (var fixedPermutation, var commonPoints) = GetCommonPoints(reference, scanningResult);

            if (fixedPermutation != null)
            {
                fixedScanningResults.Add(fixedPermutation);
                scanningResults.Remove(fixedPermutation.Original);
                allPoints.UnionWith(fixedPermutation.Positions);
                change = true;
                Console.WriteLine($"Remaining: {scanningResults.Count}");

                break;
            }
        }
        if(change)
        {
            break;
        }
    }
    if(!change)
    {
        throw new Exception("Shouldnt happen");
    }
}

//a
Console.WriteLine(allPoints.Count);
//b
var maxDistance = 0;
for (int i = 0; i < fixedScanningResults.Count; i++)
{
    for (int j = 0; j < fixedScanningResults.Count; j++)
    {
        if (i == j)
            continue;

        var dist = fixedScanningResults[i].DistanceTo(fixedScanningResults[j]);
        if(dist > maxDistance)
        {
            maxDistance = dist;
        }
    }
}

Console.WriteLine(maxDistance);

(ScanningResult? FixedPermutation, IEnumerable<Position> CommonPoints) GetCommonPoints(ScanningResult first, ScanningResult second)
{
    foreach (var p in second.GetPermutations())
    {
        foreach (var fixedPoint in p.Positions)
        {
            foreach(var fixTo in first.Positions)
            {
                var fixedScanningResult = p.FixTo(fixedPoint, fixTo);

                var commonPoints = fixedScanningResult.GetCommonPositions(first).ToList();

                if(commonPoints.Count >= 12)
                {
                    return (fixedScanningResult, commonPoints);
                }
            }
        }
    }

    return (null, new List<Position>());
}

class ScanningResult
{

    public ScanningResult(HashSet<Position> positions, ScanningResult? original = null) : this(positions, new Position(0, 0, 0), original)
    {

    }
    public ScanningResult(HashSet<Position> positions, Position scannerPosition, ScanningResult? original = null)
    {
        Positions = positions;
        ScannerPosition = scannerPosition;
        Original = original;
    }

    public HashSet<Position> Positions { get; private set; }
    public ScanningResult? Original { get; private set; }

    private IEnumerable<ScanningResult>? _permutations;

    public Position ScannerPosition { get; private set; }

    public ScanningResult FixTo(Position fixedPoint, Position fixTo)
    {
        var difference = fixTo - fixedPoint;

        var newPositions = Positions.Select(p => p.Translate(difference)).ToHashSet();

        return new ScanningResult(newPositions, difference, Original);
    }

    public IEnumerable<ScanningResult> GetPermutations()
    {
        if(_permutations == null)
        {
            _permutations = GetPermutationsInner();
        }

        return _permutations;
    }

    public int DistanceTo(ScanningResult other)
    {
        return Math.Abs(ScannerPosition.X - other.ScannerPosition.X) + Math.Abs(ScannerPosition.Y - other.ScannerPosition.Y) + Math.Abs(ScannerPosition.Z - other.ScannerPosition.Z);
    }

    public IEnumerable<Position> GetCommonPositions(ScanningResult other)
    {
        return Positions.Intersect(other.Positions);
    }

    private IEnumerable<ScanningResult> GetPermutationsInner()
    {
        for(int lookTowards = 0; lookTowards < 3; lookTowards++)
        {
            foreach(int direction in new int[] { -1, 1 })
            {
                var lookToVector = Position.Eye(direction, lookTowards);
                foreach (var upVector in Position.GetPossibleUpVectors(lookTowards))
                {
                    var rotationMatrix = new int[3, 3];
                    var xAxis = Position.CrossProduct(upVector, lookToVector);
                    var yAxis = Position.CrossProduct(lookToVector, xAxis);


                    rotationMatrix[0, 0] = xAxis.X;
                    rotationMatrix[1, 0] = xAxis.Y;
                    rotationMatrix[2, 0] = xAxis.Z;
                    rotationMatrix[0, 1] = yAxis.X;
                    rotationMatrix[1, 1] = yAxis.Y;
                    rotationMatrix[2, 1] = yAxis.Z;
                    rotationMatrix[0, 2] = lookToVector.X;
                    rotationMatrix[1, 2] = lookToVector.Y;
                    rotationMatrix[2, 2] = lookToVector.Z;

                    yield return new ScanningResult(Positions.Select(p => p.Apply(rotationMatrix)).ToHashSet(), this);
                }
            }
        }
    }
}


record Position(int X, int Y, int Z)
{
    public Position(IEnumerable<int> coords)
        :this(coords.First(), coords.Skip(1).First(), coords.Skip(2).First())
    {

    }

    public Position Translate(Position other)
    {
        return new Position(X + other.X, Y + other.Y, Z + other.Z);
    }

    public static Position operator -(Position a, Position b)
    {
        return new Position(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    }

    public static Position CrossProduct(Position a, Position b)
    {
        return new Position(a.Y * b.Z - a.Z * b.Y, a.Z * b.X - a.X * b.Z, a.X * b.Y - a.Y * b.X);
    }

    public Position Apply(int[,] matrix)
    {
        var resultCoords = new int[3];
        for(int i = 0; i < 3; i++)
        {
            var sum = 0;
            for (int j = 0; j < 3; j++)
            {
                sum += matrix[i, j] * this[j];
            }
            resultCoords[i] = sum;
        }

        return new Position(resultCoords);

    }

    public static Position Eye(int direction, int axis)
    {
        var coords = Enumerable.Repeat(direction, 3).ToList();
        for (int i = 0; i < 3; i++)
        {
            if(i != axis)
            {
                coords[i] = 0;
            }
        }

        return new Position(coords);
    }

    public static IEnumerable<Position> GetPossibleUpVectors(int fixedAxis)
    {
        for (int axis = 0; axis < 3; axis++)
        {
            if (axis == fixedAxis)
            {
                continue;
            }

            foreach (int direction in new int[] { -1, 1 })
            {
                yield return Position.Eye(direction, axis);
            }
        }
    }

    public int this[int index]
    {
        get
        {
            return index switch
            {
                0 => X,
                1 => Y,
                2 => Z,
                _ => throw new IndexOutOfRangeException()
            };
        }
    }
}