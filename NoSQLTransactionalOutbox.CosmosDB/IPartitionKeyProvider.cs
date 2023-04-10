using Microsoft.Azure.Cosmos;
using NoSQLTransactionalOutbox.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoSQLTransactionalOutbox.CosmosDB
{
    public interface IPartitionKeyProvider<TEntity> where TEntity : DomainEntity
    {
        public PartitionKey GetPartitionKey(TEntity entity);
    }
}
