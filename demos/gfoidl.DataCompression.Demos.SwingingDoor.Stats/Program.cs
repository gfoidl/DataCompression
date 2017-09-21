#define INTERPOLATE

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static System.FormattableString;

namespace gfoidl.DataCompression.Demos.SwingingDoor.Stats
{
    static class Program
    {
        static void Main()
        {
            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "data"));
            Environment.CurrentDirectory = Path.Combine(Directory.GetCurrentDirectory(), "data");

            var data          = CreateData().ToList();
            var compressed    = data.SwingingDoorCompression(0.1).ToList();
            var reconstructed = Reconstruct(data, compressed).ToList();

            new DataPointSerializer().Write("compressed.csv", compressed, header: ("x", "y"));

            Console.WriteLine($"{"Data items:",-18}{data.Count,4}");
            Console.WriteLine($"{"Compressed items:",-18}{compressed.Count,4}");

            using (StreamWriter sw = File.CreateText("result.csv"))
            {
                sw.WriteLine("# x\traw\tcompressed\terror");

                for (int i = 0; i < data.Count; ++i)
                    sw.WriteLine(Invariant($"{data[i].X}\t{data[i].Y}\t{reconstructed[i].Y}\t{data[i].Y - reconstructed[i].Y}"));
            }

            GnuPlot();
            ShowChart();
        }
        //---------------------------------------------------------------------
        private static IEnumerable<DataPoint> CreateData()
        {
            var rnd = new Random();

            for (int i = 0; i < 1_000; ++i)
            {
                double x = i * 1e-2;
                double y = 10 * Math.Log((10 - x) + 2.5) + (rnd.NextDouble() * 0.2 - 0.1) + 5 * Math.Cos((10 - x) / 10 * Math.PI);

                yield return (x, y);
            }
        }
        //---------------------------------------------------------------------
        private static IEnumerable<DataPoint> Reconstruct(IEnumerable<DataPoint> raw, IEnumerable<DataPoint> compressed)
        {
            var compressedEnum = compressed.GetEnumerator();
            var rawEnum        = raw.GetEnumerator();

#if INTERPOLATE
            double x = default;
#endif
            double y = default;

            while (compressedEnum.MoveNext() && rawEnum.MoveNext())
            {
                yield return compressedEnum.Current;
                x = compressedEnum.Current.X;
                y = compressedEnum.Current.Y;

                if (!compressedEnum.MoveNext()) yield break;

#if INTERPOLATE
                double k = (compressedEnum.Current.Y - y) / (compressedEnum.Current.X - x);
#endif

                while (rawEnum.MoveNext() && rawEnum.Current.X < compressedEnum.Current.X)
#if INTERPOLATE
                    yield return (rawEnum.Current.X, y + k * (rawEnum.Current.X - x));
#else
                    yield return (rawEnum.Current.X, y);
#endif
                yield return compressedEnum.Current;
            }
        }
        //---------------------------------------------------------------------
        private static void GnuPlot()
        {
            var gnuPlot                        = new Process();
            //gnuPlot.StartInfo.WorkingDirectory = Path.Combine(Directory.GetCurrentDirectory(), "data");
            gnuPlot.StartInfo.FileName         = "gnuplot";
            gnuPlot.StartInfo.Arguments        = "error.plt";
            gnuPlot.Start();
            gnuPlot.WaitForExit();
        }
        //---------------------------------------------------------------------
        private static void ShowChart()
        {
            var png                       = new Process();
            png.StartInfo.UseShellExecute = true;       // defaults to false in .net Core
            png.StartInfo.FileName        = "error.png";
            png.Start();
        }
    }
}