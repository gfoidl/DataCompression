namespace gfoidl.DataCompression.Internal.NoCompression
{
    internal abstract class NoCompressionIterator : DataPointIterator
    {
        protected NoCompressionIterator(Compression compression)
            : base(compression)
        { }
    }
}
