using System;
using System.Diagnostics;
using System.Linq;

namespace gfoidl.DataCompression.Perf
{
    /*
     * See csproj for version selection
     */
    static class Program
    {
        static void Main(string[] args)
        {
            var dataPoints = new DataPoint[10_000_000];
            var rnd = new Random(0);

            for (int i = 0; i < dataPoints.Length; ++i)
                dataPoints[i] = (i, rnd.NextDouble());

            var swingingDoorCompression = new SwingingDoorCompression(0.1);
            var iterator = swingingDoorCompression.Process(dataPoints);
            var array = iterator.ToArray();

            Console.WriteLine(iterator.GetType());
            Console.WriteLine($"count: {array.Length}");
            int n = 0;
#if !DEBUG
            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < 10; ++i)
            {
                iterator = swingingDoorCompression.Process(dataPoints);
                array = iterator.ToArray();
                n += array.Length;
            }
            sw.Stop();
            Console.WriteLine($"n: {n}\tTime [ms]: {sw.ElapsedMilliseconds * 0.1}");
#endif
        }
    }
}
