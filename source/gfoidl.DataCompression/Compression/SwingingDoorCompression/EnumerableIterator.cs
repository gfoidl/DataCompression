namespace gfoidl.DataCompression.Internal.SwingingDoor
{
    internal abstract class EnumerableIterator : SwingingDoorCompressionIterator
    {
        protected EnumerableIterator(SwingingDoorCompression swingingDoorCompression)
            : base(swingingDoorCompression)
        { }
    }
}
