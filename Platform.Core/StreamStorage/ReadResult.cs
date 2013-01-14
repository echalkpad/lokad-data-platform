using System.Collections.Generic;

namespace Platform.StreamStorage
{
    /// <summary>
    /// In order to reduce latency between reads, it's important to read multiple records
    /// at once. This class represents the result of multi-record read operation.
    /// </summary>
    public struct ReadResult
    {
        public readonly long NextOffset;

        public readonly ICollection<RetrievedDataRecord> Records;

        public ReadResult(long nextOffset, ICollection<RetrievedDataRecord> records) : this()
        {
            NextOffset = nextOffset;
            Records = records;
        }
    }
}