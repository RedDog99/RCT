using System.Net;
using System.Net.Sockets;
using rct;
using rct.Structs;

namespace cli;

public static class Program
{
  public static async Task Main()
  {
    var ipAddress = IPAddress.Parse("192.168.50.233");
    var ipEndPoint = new IPEndPoint(ipAddress, 8899);

    using TcpClient client = new();
    await client.ConnectAsync(ipEndPoint);

    await using var stream = client.GetStream();

    var rct = new Rct(stream);
    
    // Read the how load
    var l1 = await rct.Read(0x3A39CA2);
    var l2 = await rct.Read(0x2788928C);
    var l3 = await rct.Read(0xF0B436DD);
    Console.WriteLine($"House L1/2/3 {l1.PayloadAsFloat():0.00}/{l2.PayloadAsFloat():0.00}/{l3.PayloadAsFloat():0.00} W");

    // Show cell voltages of all cells
    var batteryMessages = new Dictionary<uint, Message>
    {
      {0, await rct.Read(0xF8C0D255)},
      {1, await rct.Read(0x8EF6FBBD)},
      {2, await rct.Read(0x69B8FF28)},
      {3, await rct.Read(0xC8609C8E)},
      {4, await rct.Read(0x1348AB07)},
      {5, await rct.Read(0x62D645D9)},
      {6, await rct.Read(0x40FF01B7)},
    };

    var batteries = batteryMessages.ToDictionary(kv => kv.Key, kv => kv.Value.PayloadAsCellDataDict());

    Console.Write($"    ");
    foreach (var cells in batteries.First().Value)
    {
      var c = cells.Value;
      Console.Write($"{cells.Key:   00} ");
    }
    Console.WriteLine("-  MIN / MAX  - Diff");

    var allMin = new List<ushort>();
    var allMax = new List<ushort>();

    foreach (var batt in batteries)
    {
      Console.Write($"B{batt.Key}: ");
      foreach (var cells in batt.Value)
      {
        var c = cells.Value;
        Console.Write($"{c.Voltage / 1000.0:F3} ");
        // Console.Write($"c.Temp:00}°C {c.Resistance:00}mO ");
      }

      var min = batt.Value.Min(c => c.Value.Voltage);
      var max = batt.Value.Max(c => c.Value.Voltage);
      Console.WriteLine($"- {min/1000.0:F3}/{max/1000.0:F3} - {(max-min)/1000.0:F3}");
      allMin.Add(min);
      allMax.Add(max);
    }
    {
      var min = allMin.Min();
      var max = allMax.Max();
      for (var i = 0; i < 145; i++)
      {
        Console.Write(" ");
      }
      Console.WriteLine($"SUM: {min / 1000.0:F3}/{max / 1000.0:F3} - {(max - min) / 1000.0:F3}");
    }

    // Now just read for 2s and prind results
    var ct = new CancellationTokenSource(2000).Token;
    await rct.Sniff((m) =>
    {
      Console.WriteLine($"Message: Len: {m.Payload.Length}, ID: {m.ObjectId}");
    }, ct);
  }
}