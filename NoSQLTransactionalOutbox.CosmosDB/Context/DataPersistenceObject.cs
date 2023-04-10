using Newtonsoft.Json;
using NoSQLTransactionalOutbox.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoSQLTransactionalOutbox.CosmosDB.Context
{
    public class DataPersistenceObject<T> where T : IEntity
    {
        [JsonProperty]
        public string Id { get; set; }

        [JsonProperty]
        public string PartitionKey { get; set; }

        [JsonProperty]
        public string EntityType { get; set; }

        public T Data { get; set; }

        [JsonProperty("_etag")]
        public string ETag { get; set; }

        public EntityState EntityState { get; set; }

        [JsonProperty]
        public int Ttl { get; set; }

        [JsonProperty("_rid")]
        public string RID { get; set; }

        [JsonProperty("_ts")]
        public string ItemLastUpdated { get; set; }

        [JsonProperty("_self")]
        public string RawUrl { get; set; }
    }
}
