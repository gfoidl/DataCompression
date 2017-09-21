using System.Collections.Generic;

namespace gfoidl.DataCompression
{
    public interface IDataCompression
    {
        IEnumerable<double> Process(IEnumerable<double> input);
    }
}