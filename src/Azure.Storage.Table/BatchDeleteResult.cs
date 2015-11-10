namespace Azure.Storage.Table
{
    public class BatchDeleteResult
    {
        public BatchDeleteResult(int batches, int deleteBatches, int entitiesDeleted)
        {
            Batches = batches;
            DeleteBatches = deleteBatches;
            EntitiesDeleted = entitiesDeleted;
        }

        public int Batches { get; private set; }

        public int DeleteBatches { get; private set; }

        public int EntitiesDeleted { get; private set; }

        public override string ToString()
        {
            return string.Format("Batches: {0}, DeletedBatches: {1}, EntitiesDeleted: {2}", Batches, DeleteBatches, EntitiesDeleted);
        }
    }
}