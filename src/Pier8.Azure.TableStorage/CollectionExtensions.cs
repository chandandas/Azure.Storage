using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Table;

namespace Azure.Storage.Table
{
    internal static class CollectionExtensions
    {
        public static List<List<T>> ChunkBy<T>(this IEnumerable<T> source, int chunkSize)
        {
            return source
                .Select((v, i) => new { Index = i, Value = v })
                .GroupBy(i => i.Index / chunkSize)
                .Select(i => i.Select(v => v.Value).ToList())
                .ToList();
        }

        public static void AddRange(this TableBatchOperation tableBatchOperation, IEnumerable<TableOperation> tableOperations)
        {
            foreach (var tableOperation in tableOperations)
            {
                tableBatchOperation.Add(tableOperation);
            }
        }
    }
}