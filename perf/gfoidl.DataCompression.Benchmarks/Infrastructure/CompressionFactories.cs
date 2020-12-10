namespace gfoidl.DataCompression.Benchmarks.Infrastructure
{
    public interface ICompressionFactory
    {
        ICompression Create();
    }
    //-------------------------------------------------------------------------
    public class DeadBandCompressionFactory : ICompressionFactory
    {
        public ICompression Create() => new DataCompression.DeadBandCompression(0.1);
    }
    //-------------------------------------------------------------------------
    public class SwingingDoorCompressionFactory : ICompressionFactory
    {
        public ICompression Create() => new DataCompression.SwingingDoorCompression(0.1);
    }
}
