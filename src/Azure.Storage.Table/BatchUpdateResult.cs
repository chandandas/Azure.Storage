namespace Azure.Storage.Table
{
    public class BatchUpdateResult
    {
        public BatchUpdateResult(int batches, int updatedBatches, int entitiesUpdated)
        {
            Batches = batches;
            UpdatedBatches = updatedBatches;
            EntitiesUpdated = entitiesUpdated;
        }

        public int Batches { get; private set; }

        public int UpdatedBatches { get; private set; }

        public int EntitiesUpdated { get; private set; }
    }
}