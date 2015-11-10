using System;
using System.Reflection;
using Microsoft.WindowsAzure.Storage.Table;

namespace Azure.Storage.Table
{
    public static class TsContextInitialiser
    {
        private static readonly MethodInfo _createTsTablehMethodInfo;

        static TsContextInitialiser()
        {
            _createTsTablehMethodInfo = typeof(TsContextInitialiser).GetMethod("CreateTsTable", BindingFlags.Static | BindingFlags.NonPublic);
        }

        public static T Create<T>() where T : TsContext
        {
            var contextType = typeof(T);
            var contextInstance = (T)Activator.CreateInstance(contextType);
            contextInstance.Initialise();

            var cloudTableClient = contextInstance.TableClient;

            foreach (var info in contextType.GetProperties())
            {
                if (info.PropertyType.IsGenericType && info.PropertyType.GetGenericTypeDefinition() == typeof(TsSet<>))
                {
                    var entityType = info.PropertyType.GetGenericArguments()[0];
                    var table = _createTsTablehMethodInfo.MakeGenericMethod(entityType).Invoke(null, new object[] { cloudTableClient, info.Name });

                    var set = Activator.CreateInstance(info.PropertyType, table);
                    info.SetValue(contextInstance, set);
                }
            }

            return contextInstance;
        }

        private static TsTable<T> CreateTsTable<T>(CloudTableClient cloudTableClient, string name) where T : TableEntity, new()
        {
            return new TsTable<T>(cloudTableClient.GetTableReference(name));
        }
    }
}