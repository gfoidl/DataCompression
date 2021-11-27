// (c) gfoidl, all rights reserved

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace gfoidl.DataCompression.Demos.LiveData
{
    static class Program
    {
        static void Main()
        {
            Environment.CurrentDirectory = Path.Combine(Directory.GetCurrentDirectory(), "data");

            Run("agt_n_awt1", 0.25, 0.5);
#if !CI_BUILD
            Console.WriteLine("hit key to continue...");
            Console.ReadKey();
#endif

            Run("agt_zyl_6", 0.75, 1.5);
#if !CI_BUILD
            Console.WriteLine("hit key to continue...");
            Console.ReadKey();
#endif

            Run("erregerspannung", 0.4, 0.8);
#if !CI_BUILD
            Console.WriteLine("hit key to continue...");
            Console.ReadKey();
#endif
        }
        //---------------------------------------------------------------------
        private static void Run(string name, double instrumentPrecision, double compressionDeviation)
        {
            var dataPointReader = new DataPointSerializer();

            IEnumerable<DataPoint> compressed = dataPointReader
                .Read($"{name}.csv", dateTimeFormat: "yyyy-MM-dd_HH:mm:ss")
                .DeadBandCompression(instrumentPrecision)
                .SwingingDoorCompression(compressionDeviation);

            dataPointReader.Write($"{name}_compressed.csv", compressed, dateTimeFormat: "yyyy-MM-dd_HH:mm:ss");

            GnuPlot(name);
            ShowChart(name);
        }
        //---------------------------------------------------------------------
        private static void GnuPlot(string name)
        {
            var gnuPlot                        = new Process();
            //gnuPlot.StartInfo.WorkingDirectory = Path.Combine(Directory.GetCurrentDirectory(), "data");
            gnuPlot.StartInfo.FileName         = "gnuplot";
            gnuPlot.StartInfo.Arguments        = $"{name}.plt";
            gnuPlot.Start();
            gnuPlot.WaitForExit();
        }
        //---------------------------------------------------------------------
        private static void ShowChart(string name)
        {
#if !SKIP_PLOT_DISPLAY
            var png                       = new Process();
            png.StartInfo.UseShellExecute = true;       // defaults to false in .NET (Core)
            png.StartInfo.FileName        = $"{name}.png";
            png.Start();
#endif
        }
    }
}
