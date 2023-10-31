namespace rct.Extensions;

public static class StreamExtensions
{
  public static async Task<int> ReadByteAsync(this Stream @this)
  {
    var buffer = new byte[1];
    var count = await @this.ReadAsync(buffer, 0, 1);
    if (count != 1)
    {
      throw new IOException("Unable to read from stream");
    }
    return buffer[0];
  }
  
  public static async Task<int> ReadEscapedByteAsync(this Stream @this)
  {
    var ret = await @this.ReadByteAsync();
    if (ret.Equals('-'))
    {
      ret = await @this.ReadByteAsync();
    }

    return ret;
  }
}