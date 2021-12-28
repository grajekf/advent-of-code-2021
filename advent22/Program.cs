using System.Text.RegularExpressions;

var instructions = File.ReadAllLines("input.txt").Select(Instruction.Parse).ToList();

var instructionsForInit = instructions.Where(i => i.IsForInitialization()).ToList();

var litCubes = new HashSet<(int X, int Y, int Z)>();

foreach (var instruction in instructionsForInit)
{
    for (int x = instruction.XFrom; x <= instruction.XTo; x++)
    {
        for (int y = instruction.YFrom; y <= instruction.YTo; y++)
        {
            for (int z = instruction.ZFrom; z <= instruction.ZTo; z++)
            {
                var index = (X: x, Y: y, Z: z);
                if(instruction.On)
                {
                    litCubes.Add(index);
                }
                else
                {
                    litCubes.Remove(index);
                }
            }
        }
    }
}

//a
Console.WriteLine(litCubes.Count);

//b
    
var litCuboids = new List<Instruction>();

foreach(var instruction in instructions)
{
    if (instruction.On)
    {
        var newCuboids = new List<Instruction>()
        {
            instruction
        };

        foreach (var cuboid in litCuboids)
        {
            newCuboids = newCuboids.SelectMany(n => n.TurnOff(cuboid).ToList()).ToList();
        }

        litCuboids.AddRange(newCuboids);
    }
    else
    {
        litCuboids = litCuboids.SelectMany(c => c.TurnOff(instruction).ToList()).ToList();
    }
}

Console.WriteLine(litCuboids.Sum(c => c.CubeCount));

class Instruction
{
    public Instruction(int xFrom, int xTo, int yFrom, int yTo, int zFrom, int zTo, bool on)
    {
        XFrom = xFrom;
        XTo = xTo;
        YFrom = yFrom;
        YTo = yTo;
        ZFrom = zFrom;
        ZTo = zTo;
        On = on;
    }

    public int XFrom { get; set; }
    public int XTo { get; set; }
    public int YFrom { get; set; }
    public int YTo { get; set; }
    public int ZFrom { get; set; }
    public int ZTo { get; set; }
    public bool On { get; set; }

    public long CubeCount
    {
        get
        {
            return (long)(XTo - XFrom + 1) * (YTo - YFrom + 1) * (ZTo - ZFrom + 1);
        }
    }

    public bool IsForInitialization()
    {
        return XFrom >= -50 && YFrom >= -50 && ZFrom >= -50 && XTo <= 50 && YTo <= 50 && ZTo <= 50;
    }

    public bool SamePosition(Instruction other)
    {
        return other.XFrom == XFrom && other.YFrom == YFrom && other.ZFrom == ZFrom
            && other.XTo == XTo && other.YTo == YTo && other.ZTo == ZTo;
    }

    public bool IsDegenerate()
    {
        return XFrom > XTo || YFrom > YTo || ZFrom > ZTo;
    }

    public IEnumerable<Instruction> TurnOff(Instruction other)
    {
        var intersectFromX = Math.Max(XFrom, other.XFrom);
        var intersectFromY = Math.Max(YFrom, other.YFrom);
        var intersectFromZ = Math.Max(ZFrom, other.ZFrom);

        var intersectToX = Math.Min(XTo, other.XTo);
        var intersectToY = Math.Min(YTo, other.YTo);
        var intersectToZ = Math.Min(ZTo, other.ZTo);

        if(intersectFromX > intersectToX || intersectFromY > intersectToY || intersectFromZ > intersectToZ)
        {
            yield return this;
            yield break;
        }

        var intersectionInstruction = new Instruction(intersectFromX, intersectToX, intersectFromY, intersectToY, intersectFromZ, intersectToZ, false);

        if (SamePosition(intersectionInstruction))
        {
            yield break;
        }

        var allX = new List<int>()
        {
            XFrom, intersectFromX - 1, intersectFromX, intersectToX, intersectToX + 1, XTo
        };

        var allY = new List<int>()
        {
            YFrom, intersectFromY - 1, intersectFromY, intersectToY, intersectToY + 1, YTo
        };

        var allZ = new List<int>()
        {
            ZFrom, intersectFromZ - 1, intersectFromZ, intersectToZ, intersectToZ + 1, ZTo
        };

        for (int x = 0; x < allX.Count; x+=2)
        {
            for (int y = 0; y < allY.Count; y+=2)
            {
                for (int z = 0; z < allZ.Count; z+=2)
                {
                    var newInstruction = new Instruction(allX[x], allX[x+1], allY[y], allY[y+1], allZ[z], allZ[z+1], true);
                    if (!newInstruction.SamePosition(intersectionInstruction) && !newInstruction.IsDegenerate())
                    {
                        yield return newInstruction;
                    }
                }
            }
        }
    }

    public static Instruction Parse(string input)
    {
        var match = Regex.Match(input, @"^(?<OnOff>on|off) x=(?<XFrom>-?\d+)\.\.(?<XTo>-?\d+),y=(?<YFrom>-?\d+)\.\.(?<YTo>-?\d+),z=(?<ZFrom>-?\d+)\.\.(?<ZTo>-?\d+)$");

        var onOff = match.Groups["OnOff"].Value == "on";
        var xFrom = int.Parse(match.Groups["XFrom"].Value);
        var xTo = int.Parse(match.Groups["XTo"].Value);
        var yFrom = int.Parse(match.Groups["YFrom"].Value);
        var yTo = int.Parse(match.Groups["YTo"].Value);
        var zFrom = int.Parse(match.Groups["ZFrom"].Value);
        var zTo = int.Parse(match.Groups["ZTo"].Value);

        return new Instruction(xFrom, xTo, yFrom, yTo, zFrom, zTo, onOff);
    }
}