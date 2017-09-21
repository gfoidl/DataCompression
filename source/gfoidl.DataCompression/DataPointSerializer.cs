using System.Collections.Generic;
using System.Globalization;
using System.IO;
using static System.FormattableString;

namespace gfoidl.DataCompression
{
    /// <summary>
    /// A simple serializer for <see cref="DataPoint" />.
    /// </summary>
    public class DataPointSerializer
    {
        /// <summary>
        /// Reads <see cref="DataPoint" />s from a csv file.
        /// </summary>
        /// <param name="csvFile">The csv file</param>
        /// <param name="separator">The delimiter in the csv file. Defaults to TAB</param>
        /// <param name="firstLineIsHeader">When <c>true</c> the first line is skipped</param>
        /// <returns>The <see cref="DataPoint" />s read from the csv file</returns>
        public IEnumerable<DataPoint> Read(string csvFile, char? separator = null, bool firstLineIsHeader = true)
        {
            char sep = separator ?? '\t';

            using (StreamReader sr = File.OpenText(csvFile))
            {
                if (firstLineIsHeader) sr.ReadLine();

                while (!sr.EndOfStream)
                {
                    string[] cols = sr.ReadLine().Split(sep);

                    double x = double.Parse(cols[0], NumberStyles.Any, CultureInfo.InvariantCulture);
                    double y = double.Parse(cols[1], NumberStyles.Any, CultureInfo.InvariantCulture);

                    yield return (x, y);
                }
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Writes <see cref="DataPoint" />s to a csv file.
        /// </summary>
        /// <param name="csvFile">The csv file</param>
        /// <param name="data">The <see cref="DataPoint" />s to write</param>
        /// <param name="separator">The delimiter in the csv file. Defaults to TAB</param>
        /// <param name="header">The header to use, or <c>null</c> to omit any header</param>
        public void Write(string csvFile, IEnumerable<DataPoint> data, char? separator = null, (string X, string Y)? header = null)
        {
            char sep = separator ?? '\t';

            using (StreamWriter sw = File.CreateText(csvFile))
            {
                if (header.HasValue)
                    sw.WriteLine($"# {header.Value.X}{sep}{header.Value.Y}");

                foreach (DataPoint dp in data)
                    sw.WriteLine(Invariant($"{dp.X}{sep}{dp.Y}"));
            }
        }
    }
}