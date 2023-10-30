using rct.Enums;
using rct.Extensions;
using rct.Structs;

namespace rct;

public class MessageReader
{
  private readonly Stream _stream;

  public MessageReader(Stream stream)
  {
    _stream = stream;
  }

  public async Task<Message> GetNextMessage()
  {
    while (await _stream.ReadByteAsync() != '+')
    {
    }

    var cmd = (Command)await _stream.ReadByteAsync();

    var payloadLen = await _stream.ReadEscapedByteAsync();
    if (cmd.IsLongCommand())
    {
      payloadLen = (payloadLen << 8) + await _stream.ReadEscapedByteAsync();
    }

    CrcCalculation crcCalc = new(cmd, payloadLen);

    uint? plantAddress = null;
    if (cmd.IsPlantCommand())
    {
      plantAddress = 0;
      for (int i = 0; i < 4; i++)
      {
        var read = (byte)await _stream.ReadEscapedByteAsync();
        plantAddress = (plantAddress << 8) + read;
        crcCalc.Append(read);
      }
    }

    uint objectId = 0;
    if (payloadLen > 4)
    {
      for (int i = 0; i < 4; i++)
      {
        var read = (byte)await _stream.ReadEscapedByteAsync();
        objectId = (objectId << 8) + read;
        crcCalc.Append(read);
      }

      payloadLen -= 4;
    }

    var payload = new byte[payloadLen];
    for (int received = payloadLen - 1; received >= 0; received--)
    {
      var read = (byte)await _stream.ReadEscapedByteAsync();
      payload[received] = read;
      crcCalc.Append(read);
    }

    ushort crc = 0;
    for (int i = 0; i < 2; i++)
    {
      var read = (byte)await _stream.ReadEscapedByteAsync();
      crc = (ushort)((crc << 8) + read);
    }

    if (crc != crcCalc.GetCrc())
    {
      throw new Exception("CRC Error");
    }

    return new Message
    {
      Command = cmd,
      ObjectId = objectId,
      Address = plantAddress,
      Payload = payload,
    };
  }
}