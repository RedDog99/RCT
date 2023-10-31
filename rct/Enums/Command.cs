namespace rct.Enums;

public enum Command
{
  Read = 0x01, // Request the current value of an object ID. No payload.
  Write = 0x02, // Write the payload to the object ID.
  LongWrite = 0x03, // When writing "long" payloads.
  Response = 0x05, // Normal response to a read or write command.
  LongResponse = 0x06, // Response with a "long" payload.
  ReadPeriodically = 0x08, // Request automatic, periodic sending of an OIDs value.
  PlantRead = 0x41, // READ for plant communication.
  PlantWrite = 0x42, // WRITE for plant communication.
  PlantLongWrite = 0x43, // LONG_WRITE for plant communication.
  PlantResponse = 0x45, // RESPONSE for plant communication.
  PlantLongResponse = 0x46, // LONG_RESPONSE for plant communication.
  PlantReadPeriodically = 0x48, // READ_PERIODICALLY for plant communication.
  Extension = 0x3c, // Unknown, see below.
}