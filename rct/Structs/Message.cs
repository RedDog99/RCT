using rct.Enums;

namespace rct.Structs;

public struct Message
{
  public Command Command;
  public ushort Length => (ushort)(4 + Payload.Length);
  public uint? Address; // only for plant communication
  public uint ObjectId; // only for plant communication
  public byte[] Payload;

  public float PayloadAsFloat()
  {
    return BitConverter.ToSingle(Payload, 0);
  }

  public bool PayloadAsBool()
  {
    return BitConverter.ToBoolean(Payload, 0);
  }

  public int PayloadAsInt()
  {
    return Payload.Length switch
    {
      1 => Payload[0],
      2 => BitConverter.ToInt16(Payload, 0),
      4 => BitConverter.ToInt32(Payload, 0),
      _ => throw new InvalidCastException("Invalid payload")
    };
  }
  
  public uint PayloadAsUInt()
  {
    return Payload.Length switch
    {
      1 => Payload[0],
      2 => BitConverter.ToUInt16(Payload, 0),
      4 => BitConverter.ToUInt32(Payload, 0),
      _ => throw new InvalidCastException("Invalid payload")
    };
  }

  public string PayloadAsString()
  {
    return BitConverter.ToString(Payload, 0);
  }

  public Dictionary<uint, CellData> PayloadAsCellDataDict()
  {
    if (Payload.Length != 96)
    {
      throw new InvalidCastException("Invalid payload");
    }

    var result = new Dictionary<uint, CellData>();
    for (uint i = 0; i < Payload.Length; i += 4)
    {
      result.Add(i / 4, new CellData
      {
        Temp = Payload[i + 3],
        Voltage = (ushort)((Payload[i + 1] << 8) + Payload[i + 2]),
        Resistance = Payload[i],
      });
    }

    return result;
  }

  // DataType.ENUM:
  // DataType.EVENT_TABLE:
  // DataType.TIMESERIES:
  // DataType.UNKNOWN:
}