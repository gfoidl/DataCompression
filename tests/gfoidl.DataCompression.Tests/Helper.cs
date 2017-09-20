using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace gfoidl.DataCompression.Tests
{
    public static class Helper
    {
        public static IEnumerable<DataPoint> ReadDataPointsFromFile(string fileName)
        {
            using (StreamReader sr = File.OpenText(fileName))
            {
                sr.ReadLine();      // header

                while (!sr.EndOfStream)
                {
                    string[] cols = sr.ReadLine().Split(' ', '\t');

                    double x = double.Parse(cols[0]);
                    double y = double.Parse(cols[1], NumberStyles.Any, CultureInfo.InvariantCulture);

                    yield return (x, y);
                }
            }
        }
    }
}