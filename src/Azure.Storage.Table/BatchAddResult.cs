namespace Azure.Storage.Table
{
    public class BatchAddResult
    {
        public BatchAddResult(int batches, int entitiesCreated)
        {
            Batches = batches;
            EntitiesCreated = entitiesCreated;
        }

        public int Batches { get; private set; }

        public int EntitiesCreated { get; private set; }

        public override string ToString()
        {
            return string.Format("Batches: {0}, EntitiesCreated: {1}", Batches, EntitiesCreated);
        }
    }
}