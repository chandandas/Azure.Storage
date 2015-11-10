using System.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Azure.Storage.Table
{
    public class TsContext
    {
        private readonly string _nameOrConnectionString;

        public TsContext(string nameOrConnectionString)
        {
            _nameOrConnectionString = nameOrConnectionString;
        }

        public CloudTableClient TableClient { get; private set; }

        public void Initialise()
        {
            TableClient = InitialiseTableClient();
        }

        private CloudTableClient InitialiseTableClient()
        {
            var storageAccount = CloudStorageAccount.Parse(GetConnectionString(_nameOrConnectionString));
            return storageAccount.CreateCloudTableClient();
        }

        private string GetConnectionString(string nameOrConnectionString)
        {
            return IsConnectionString(nameOrConnectionString)
                ? nameOrConnectionString
                : GetConnectionStringFromConfig(nameOrConnectionString);
        }

        private bool IsConnectionString(string nameOrConnectionString)
        {
            return nameOrConnectionString.ToLower().StartsWith("defaultendpointsprotocol")
                   || nameOrConnectionString.ToLower().StartsWith("usedevelopmentstorage");
        }

        private string GetConnectionStringFromConfig(string nameOrConnectionString)
        {
            return ConfigurationManager
                .ConnectionStrings[nameOrConnectionString]
                .ConnectionString;
        }
    }
}