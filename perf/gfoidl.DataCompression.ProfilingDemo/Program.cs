using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace gfoidl.DataCompression.ProfilingDemo
{
    static class Program
    {
        private static readonly DataPoint[] s_dataPoints;
        private static List<DataPoint>      s_compressed = new List<DataPoint>(1000);
        //---------------------------------------------------------------------
        static Program()
        {
            Random rnd   = new Random();
            s_dataPoints = new DataPoint[1000];

            for (int i = 0; i < s_dataPoints.Length; ++i)
            {
                s_dataPoints[i] = new DataPoint(i, rnd.NextDouble());
            }
        }
        //---------------------------------------------------------------------
        static async Task Main()
        {
            using CancellationTokenSource cts = new CancellationTokenSource();
            int counter = 0;

            Task t = Task.Run(() =>
            {
                // A single instance can be re-used over and over again
                DeadBandCompression deadBand         = new DeadBandCompression(0.1);
                SwingingDoorCompression swingingDoor = new SwingingDoorCompression(0.1);

                while (!cts.IsCancellationRequested)
                {
                    s_compressed.Clear();
                    counter++;

                    // Allocates a new instance of compression each iteration
                    //DataPointIterator compressed = s_dataPoints//.Select(d => d)
                    //    //.DeadBandCompression(0.05)
                    //    .SwingingDoorCompression(0.1);

                    //DataPointIterator compressed = compression.Process(s_dataPoints.Select(d => d));
                    DataPointIterator deadBandCompressed = deadBand.Process(s_dataPoints);
                    DataPointIterator swingingDoorCompressed = swingingDoor.Process(deadBandCompressed);

                    foreach (DataPoint item in swingingDoorCompressed)
                    {
                        s_compressed.Add(item);
                    }

                    Console.Write(".");
                }
            }, cts.Token);

            Console.WriteLine("any key to stop...");
            Console.ReadKey();

            cts.Cancel();

            try
            {
                await t;
            }
            catch (OperationCanceledException)
            { }

            Console.WriteLine($"\n\nend -- counter: {counter}");
        }
    }
}
