namespace Azure.Storage.Table
{
    public class BatchProcessResult
    {
        public BatchProcessResult(int batches, int entitiesProcessed)
        {
            Batches = batches;
            EntitiesProcessed = entitiesProcessed;
        }

        public int Batches { get; private set; }

        public int EntitiesProcessed { get; private set; }

        public override string ToString()
        {
            return string.Format("Batches: {0}, EntitiesProcessed: {1}", Batches, EntitiesProcessed);
        }
    }
}