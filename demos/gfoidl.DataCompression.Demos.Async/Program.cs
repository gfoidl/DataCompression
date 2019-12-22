using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace gfoidl.DataCompression.Demos.Async
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var cts                            = new CancellationTokenSource();
            IAsyncEnumerable<DataPoint> source = Source(cts.Token);
            //DataPointIterator filtered         = source.DeadBandCompressionAsync(0.01, ct: cts.Token);
            DataPointIterator filtered         = source.SwingingDoorCompressionAsync(0.01, ct: cts.Token);
            ValueTask<double> sinkTask         = Sink(filtered, cts.Token);

            Console.WriteLine("any key to stop...");
            Console.ReadKey();

            cts.Cancel();
            double sum = default;
            try
            {
                sum = await sinkTask;
            }
            catch (OperationCanceledException) { }

            Console.WriteLine($"sum: {sum}");
        }
        //---------------------------------------------------------------------
        private static async IAsyncEnumerable<DataPoint> Source([EnumeratorCancellation] CancellationToken ct = default)
        {
            int i   = 0;
            var rnd = new Random(42);

            while (!ct.IsCancellationRequested)
            {
                await Task.Yield();
                double x = i++;
                double y = rnd.NextDouble();

                yield return new DataPoint(x, y);
            }
        }
        //---------------------------------------------------------------------
        private static async ValueTask<double> Sink(DataPointIterator data, CancellationToken ct = default)
        {
            double sum = 0;
            uint count = 0;

            await foreach (DataPoint dp in data.WithCancellation(ct))
            {
                sum += dp.Y;

                if (count % (1 << 15) == 0)
                {
                    Console.WriteLine($"count: {count}, sum: {sum}");
                }
                count++;
            }

            return sum;
        }
    }
}
