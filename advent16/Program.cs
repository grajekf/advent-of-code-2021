using System.Text;

var transmission = File.ReadAllText("input.txt");

var sb = new StringBuilder();

foreach (var c in transmission)
{
    var intValue = Convert.ToInt32(c.ToString(), 16);
    sb.Append(Convert.ToString(intValue, 2).PadLeft(4, '0'));
}

var binaryTransmission = sb.ToString();

(var rootPacket, _) = Packet.Parse(binaryTransmission, 0);

rootPacket.Print();
//a
Console.WriteLine(rootPacket.VersionSum());
//b
Console.WriteLine(rootPacket.Calculate());


abstract class Packet
{
    protected Packet(int version, int typeId)
    {
        Version = version;
        TypeId = typeId;
    }

    public int Version { get; set; }
    public int TypeId { get; set; }

    public static (Packet Packet, int ParserPosition) Parse(string binaryInput, int parserPosition)
    {
        var version = Convert.ToInt32(binaryInput.Substring(parserPosition, 3), 2);
        var typeId = Convert.ToInt32(binaryInput.Substring(parserPosition + 3, 3), 2);

        switch(typeId)
        {
            case 4:
                return LiteralValuePacket.Parse(version, typeId, binaryInput, parserPosition + 6);
            default:
                return OperatorPacket.Parse(version, typeId, binaryInput, parserPosition + 6);
        }
    }

    public abstract void Print(int level = 0);
    public abstract int VersionSum();
    public abstract long Calculate();
}


class LiteralValuePacket : Packet
{
    protected LiteralValuePacket(long value, int version, int typeId) : base(version, typeId)
    {
        Value = value;
    }

    public static (Packet Packet, int ParserPosition) Parse(int version, int typeId, string binaryInput, int parserPosition)
    {
        string value = string.Empty;
        while (true)
        {
            var precedingChar = binaryInput[parserPosition];
            value += binaryInput.Substring(parserPosition + 1, 4);
            parserPosition += 5;

            if(precedingChar == '0')
            {
                break;
            }
        }

        return (new LiteralValuePacket(Convert.ToInt64(value, 2), version, typeId), parserPosition);
    }

    public override void Print(int level = 0)
    {
        for(int i = 0; i < level; i++)
        {
            Console.Write('\t');
        }
        Console.WriteLine($"Literal, Version {Version}, TypeId {TypeId}, Value {Value}");
    }

    public override int VersionSum()
    {
        return Version;
    }

    public override long Calculate()
    {
        return Value;
    }

    public long Value { get; set; }
}

class OperatorPacket : Packet
{
    protected OperatorPacket(int lengthTypeId, int lenght, IEnumerable<Packet> subPackets, int version, int typeId) : base(version, typeId)
    {
        LengthTypeId = lengthTypeId;
        SubPackets = subPackets;
        Length = lenght;
    }

    public static (Packet Packet, int ParserPosition) Parse(int version, int typeId, string binaryInput, int parserPosition)
    {
        var lengthTypeId = int.Parse(binaryInput.Substring(parserPosition, 1));
        parserPosition++;

        var subPackets = new List<Packet>();

        int length;
        if (lengthTypeId == 0)
        {
            var subPacketLength = Convert.ToInt32(binaryInput.Substring(parserPosition, 15), 2);
            length = subPacketLength;
            parserPosition += 15;

            var positionAfterParse = parserPosition + subPacketLength + 1;
            while (parserPosition + 6 < positionAfterParse)
            {
                (var subPacket, parserPosition) = Packet.Parse(binaryInput, parserPosition);
                subPackets.Add(subPacket);
            }


        }
        else
        {
            var subPacketCount = Convert.ToInt32(binaryInput.Substring(parserPosition, 11), 2);
            length = subPacketCount;
            parserPosition += 11;

            for (int i = 0; i < subPacketCount; i++)
            {
                (var subPacket, parserPosition) = Packet.Parse(binaryInput, parserPosition);
                subPackets.Add(subPacket);
            }
        }

        return (new OperatorPacket(lengthTypeId, length, subPackets, version, typeId), parserPosition);
    }

    public override void Print(int level = 0)
    {
        for (int i = 0; i < level; i++)
        {
            Console.Write('\t');
        }
        Console.WriteLine($"Operator, Version {Version}, TypeId {TypeId}, LengthTypeId {LengthTypeId}, Length {Length}");

        foreach (var packet in SubPackets)
        {
            packet.Print(level + 1);
        }
    }

    public override int VersionSum()
    {
        return Version + SubPackets.Select(p => p.VersionSum()).Sum();
    }

    public override long Calculate()
    {
        switch(TypeId)
        {
            case 0:
                return Sum();
            case 1:
                return Product();
            case 2:
                return Minimum();
            case 3:
                return Maximum();
            case 5:
                return Greater();
            case 6:
                return Less();
            case 7:
                return Equal();
            default:
                throw new Exception("Unrecognized type id");
        }
    }

    public long Sum()
    {
        return SubPackets.Sum(p => p.Calculate());
    }

    public long Product()
    {
        if(SubPackets.Count() == 1)
        {
            return SubPackets.First().Calculate();
        }
        return SubPackets.Aggregate(1L, (r, p) => r * p.Calculate());
    }

    public long Minimum()
    {
        return SubPackets.Min(p => p.Calculate());
    }

    public long Maximum()
    {
        return SubPackets.Max(p => p.Calculate());
    }

    public long Greater()
    {
        if(SubPackets.First().Calculate() > SubPackets.Skip(1).First().Calculate())
        {
            return 1;
        }

        return 0;
    }

    public long Less()
    {
        if (SubPackets.First().Calculate() < SubPackets.Skip(1).First().Calculate())
        {
            return 1;
        }

        return 0;
    }

    public long Equal()
    {
        if (SubPackets.First().Calculate() == SubPackets.Skip(1).First().Calculate())
        {
            return 1;
        }

        return 0;
    }

    public int LengthTypeId { get; set; }
    public int Length { get; set; }
    public IEnumerable<Packet> SubPackets { get; set; }
}