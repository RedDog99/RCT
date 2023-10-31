using rct.Extensions;
using NullFX.CRC;
using rct.Enums;

namespace rct;

public class CrcCalculation
{
  private readonly byte[] _buffer;
  private int _index;
  private readonly bool _isUneven;

  public CrcCalculation(Command cmd, int payloadLen)
  {
    var len = 1 + (cmd.IsLongCommand() ? 2 : 1) + (cmd.IsPlantCommand() ? 4 : 0) + payloadLen;
    
    if (len % 2 == 1)
    {
      _isUneven = true;
      len++;
    }
    else
    {
      _isUneven = false;
    }

    _buffer = new byte[len];
    _index = 0;

    Append((byte)cmd);
    if (cmd.IsLongCommand())
    {
      Append((byte)(payloadLen >> 8));
    }

    Append((byte)payloadLen);
  }

  public void Append(byte data)
  {
    _buffer[_index++] = data;
  }

  public ushort GetCrc()
  {
    if (_isUneven)
    {
      Append(0);
    }

    return Crc16.ComputeChecksum(Crc16Algorithm.CcittInitialValue0xFFFF, _buffer, 0, _buffer.Length);
  }
}