using System.Net;
using System.Net.Sockets;
using rct;

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
    
    var l1 = await rct.Read(0x3A39CA2);
    var l2 = await rct.Read(0x2788928C);
    var l3 = await rct.Read(0xF0B436DD);
    Console.WriteLine($"House L1/2/3 {l1.PayloadAsFloat():0.00}/{l2.PayloadAsFloat():0.00}/{l3.PayloadAsFloat():0.00} W");
  }
}