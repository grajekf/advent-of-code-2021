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

        //while(parserPosition % 4 != 0)
        //{
        //    parserPosition++;
        //}

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
        var length = 0;

        parserPosition++;

        var subPackets = new List<Packet>();

        if(lengthTypeId == 0)
        {
            var subPacketLength = Convert.ToInt32(binaryInput.Substring(parserPosition, 15), 2);
            length = subPacketLength;
            parserPosition += 15;
            //var subPacketString = binaryInput.Substring(parserPosition, subPacketLength);
            //parserPosition += subPacketLength;
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
            //var subPacketString = binaryInput.Substring(parserPosition);

            for (int i = 0; i < subPacketCount; i++)
            {
                (var subPacket, parserPosition) = Packet.Parse(binaryInput, parserPosition);
                subPackets.Add(subPacket);
            }
        }

        //while (parserPosition % 4 != 0)
        //{
        //    parserPosition++;
        //}

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

    public int LengthTypeId { get; set; }
    public int Length { get; set; }
    public IEnumerable<Packet> SubPackets { get; set; }
}