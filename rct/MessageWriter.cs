using rct.Extensions;
using rct.Structs;

namespace rct;

public class MessageWriter
{
  private readonly Stream _stream;

  public MessageWriter(Stream stream)
  {
    _stream = stream;
  }
  
  private static IEnumerable<byte> Escape(List<byte> data)
  {
    var ret = new List<byte>();
    foreach (var d in data)
    {
      if (d == '-' || d == '+')
      {
        ret.Add((byte)'-');
      }

      ret.Add(d);
    }

    return ret;
  }


  public async Task WriteMessage(Message m)
  {
    //  length without start
    var msgLen = 1 /*Command*/ + (m.Command.IsLongCommand() ? 2 : 1) +
                 (m.Command.IsPlantCommand() ? 4 : 0) + m.Length + 2 /*crc*/;
    var buffer = new List<byte>();

    buffer.Add((byte)m.Command);
    if (m.Command.IsLongCommand())
    {
      buffer.Add((byte)(m.Length >> 8));
    }
    buffer.Add((byte)m.Length);

    var crcCalc = new CrcCalculation(m.Command, m.Length);

    void Append32Bit(uint value)
    {
      for (var i = 3; i >= 0; i--)
      {
        var tmp = (byte)(value >> (i * 8));
        buffer.Add(tmp);
        crcCalc.Append(tmp);
      }
    }

    if (m.Command.IsPlantCommand() && m.Address.HasValue)
    {
      Append32Bit(m.Address.Value);
    }

    Append32Bit(m.ObjectId);

    if (m.Command.HasPayload())
    {
      foreach (var p in m.Payload)
      {
        buffer.Add(p);
        crcCalc.Append(p);
      }
    }

    var crc = crcCalc.GetCrc();
    for (var i = 1; i >= 0; i--)
    {
      var tmp = (byte)(crc >> (i * 8));
      buffer.Add(tmp);
    }

    if (buffer.Count != msgLen)
    {
      throw new Exception("Message generation failed.");
    }

    var escaped = Escape(buffer);
    var outBuf = escaped.Prepend((byte)'+').ToArray();

    await _stream.WriteAsync(outBuf);
  }
}