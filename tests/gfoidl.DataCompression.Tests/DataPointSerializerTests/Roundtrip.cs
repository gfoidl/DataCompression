using System.IO;
using System.Linq;
using NUnit.Framework;

namespace gfoidl.DataCompression.Tests.DataPointSerializerTests
{
    [TestFixture]
    public class Roundtrip
    {
        [Test]
        public void Write_Read_roundtrip___OK([Values(true, false)] bool writeHeader)
        {
            DataPoint[] dataPoints =
            {
                DataPoint.Origin,
                new DataPoint(1, 2),
                new DataPoint(2, -1)
            };

            var sut     = new DataPointSerializer();
            string file = Path.GetTempFileName();

            if (writeHeader)
            {
                sut.Write(file, dataPoints, header: ("x", "y"));
            }
            else
            {
                sut.Write(file, dataPoints);
            }
            DataPoint[] actual = sut.Read(file, firstLineIsHeader: writeHeader).ToArray();

            CollectionAssert.AreEqual(dataPoints, actual);
        }
    }
}
