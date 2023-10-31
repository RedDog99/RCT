using rct.Enums;

namespace rct.Extensions;

public static class CommandExtensions
{
  public static bool HasPayload(this Command cmd)
  {
    return cmd != Command.Read;
  }

  public static bool IsLongCommand(this Command cmd)
  {
    return cmd is Command.LongWrite or Command.LongResponse or Command.PlantLongWrite or Command.PlantLongResponse;
  }

  public static bool IsPlantCommand(this Command cmd)
  {
    return cmd is Command.PlantRead or Command.PlantResponse or Command.PlantWrite or Command.PlantLongResponse
      or Command.PlantLongWrite or Command.PlantReadPeriodically;
  }
}