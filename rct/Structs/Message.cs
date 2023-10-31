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

  // DataType.ENUM:
  // DataType.EVENT_TABLE:
  // DataType.TIMESERIES:
  // DataType.UNKNOWN:
}