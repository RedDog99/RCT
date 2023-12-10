namespace rct.Exceptions;

public class CrcException : Exception
{
  public CrcException(string? message)
    : base(message)
  {
  }

  public CrcException(string? message, Exception? innerException)
    : base(message, innerException)
  {
  }
}