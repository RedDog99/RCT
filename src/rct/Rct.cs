using rct.Enums;
using rct.Structs;

namespace rct;

public class Rct
{
  private readonly MessageReader _reader;
  private readonly MessageWriter _writer;

  public Rct(Stream stream)
  {
    _reader = new MessageReader(stream);
    _writer = new MessageWriter(stream);
  }

  public async Task<Message> Read(uint objectId, CancellationToken cancel = default)
  {
    var request = new Message{ Command = Command.Read, ObjectId = objectId, Payload = Array.Empty<byte>()};
    await _writer.WriteMessage(request);
    do
    {
      var m= await _reader.GetNextMessage();
      if (m.ObjectId == objectId)
      {
        return m;
      }
    } while (!cancel.IsCancellationRequested);

    throw new OperationCanceledException();
  }
}