using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace gfoidl.DataCompression.Demos.DeadBand
{
    static class Program
    {
        static void Main()
        {
            Environment.CurrentDirectory = Path.Combine(Directory.GetCurrentDirectory(), "data");

            var dataPointReader = new DataPointSerializer();

            IEnumerable<DataPoint> compressed = dataPointReader
                .Read("coolant-temp.csv")
                .DeadBandCompression(1d);

            dataPointReader.Write("coolant-temp_compressed.csv", compressed, header: ("Oh", "Temp"));

            GnuPlot();
            ShowChart();
        }
        //---------------------------------------------------------------------
        private static void GnuPlot()
        {
            var gnuPlot                        = new Process();
            //gnuPlot.StartInfo.WorkingDirectory = Path.Combine(Directory.GetCurrentDirectory(), "data");
            gnuPlot.StartInfo.FileName         = "gnuplot";
            gnuPlot.StartInfo.Arguments        = "coolant-temp.plt";
            gnuPlot.Start();
            gnuPlot.WaitForExit();
        }
        //---------------------------------------------------------------------
        private static void ShowChart()
        {
            var png                       = new Process();
            png.StartInfo.UseShellExecute = true;       // defaults to false in .net Core
            png.StartInfo.FileName        = "coolant-temp.png";
            png.Start();
        }
    }
}