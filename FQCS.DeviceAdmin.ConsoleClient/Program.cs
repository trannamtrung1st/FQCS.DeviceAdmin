using System;

namespace FQCS.DeviceAdmin.ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var dt = DateTime.UtcNow;
            var unix = dt.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
            Console.WriteLine(dt);
            Console.WriteLine(unix);
        }
    }
}
