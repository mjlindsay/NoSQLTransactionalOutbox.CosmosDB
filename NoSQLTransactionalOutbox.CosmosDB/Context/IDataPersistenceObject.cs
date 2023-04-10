using Newtonsoft.Json;
using NoSQLTransactionalOutbox.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoSQLTransactionalOutbox.CosmosDB.Context
{
    public interface IDataPersistenceObject<out T> where T : IEntity
    {
        [JsonProperty]
        public string Id { get; }

        [JsonProperty]
        public string PartitionKey { get; }

        [JsonProperty]
        public string EntityType { get; }

        T Data { get; }

        [JsonProperty("_etag")]
        public string ETag { get; set; }

        public EntityState EntityState { get; set; }

        [JsonProperty]
        public int Ttl { get; }

        [JsonProperty("_rid")]
        public string RID { get; }

        [JsonProperty("_ts")]
        public string ItemLastUpdated { get; }

        [JsonProperty("_self")]
        public string RawUrl { get; }
    }
}
