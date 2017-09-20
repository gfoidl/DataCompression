using System.Collections.Generic;

namespace gfoidl.DataCompression
{
    public class NoCompression : Compression
    {
        protected override IEnumerable<DataPoint> ProcessCore(IEnumerable<DataPoint> data) => data;
    }
}