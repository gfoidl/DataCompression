using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace gfoidl.DataCompression.Tests.DataPointSerializerTests
{
    [TestFixture]
    public class Roundtrip
    {
        private string _tmpFile;
        //---------------------------------------------------------------------
        [SetUp]
        public void SetUp() => _tmpFile = Path.GetTempFileName();
        //---------------------------------------------------------------------
        [TearDown]
        public void TearDown()
        {
            try
            {
                File.Delete(_tmpFile);
            }
            catch { }
        }
        //---------------------------------------------------------------------
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
            string file = _tmpFile;

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
        //---------------------------------------------------------------------
        [Test]
        public void Write_Read_roundtrip_with_datetime___OK([Values(true, false)] bool writeHeader)
        {
            DateTime now   = new DateTime(2019, 12, 16, 21, 57, 13);
            DateTime[] dts =
            {
                now.AddMinutes(-12),
                now
            };

            DataPoint[] dataPoints =
            {
                new DataPoint(dts[0],  2),
                new DataPoint(dts[1], -1)
            };

            var sut     = new DataPointSerializer();
            string file = _tmpFile;

            const string datetimeFormat = "yyyy-MM-dd:HHmmss";

            if (writeHeader)
            {
                sut.Write(file, dataPoints, header: ("x", "y"), dateTimeFormat: datetimeFormat);
            }
            else
            {
                sut.Write(file, dataPoints, dateTimeFormat: datetimeFormat);
            }
            DataPoint[] actual = sut.Read(file, firstLineIsHeader: writeHeader, dateTimeFormat: datetimeFormat).ToArray();

            Assert.Multiple(() =>
            {
                Assert.AreEqual(2, actual.Length);

                Assert.AreEqual(dts[0], new DateTime((long)(actual[0].X)));
                Assert.AreEqual(dataPoints[0].Y, actual[0].Y, 1e-3);

                Assert.AreEqual(dts[1], new DateTime((long)(actual[1].X)));
                Assert.AreEqual(dataPoints[1].Y, actual[1].Y, 1e-3);
            });
        }
    }
}
