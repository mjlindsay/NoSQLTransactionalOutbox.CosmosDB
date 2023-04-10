using NoSQLTransactionalOutbox.Core.Entity;
using NoSQLTransactionalOutbox.CosmosDB.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoSQLTransactionalOutbox.CosmosDB
{
    public interface IRepository<TEntity> where TEntity : DomainEntity
    {
        public void Create(TEntity entity);

        public Task<DataPersistenceObject<TEntity>> CreateAsync(TEntity entity);

        public Task<DataPersistenceObject<TEntity>> ReadAsync(string id, string? etag = null);

        public Task<DataPersistenceObject<TEntity>> DeleteAsync(string id, string? etag = null);

        public Task<IEnumerable<DataPersistenceObject<TEntity>>> ReadAllAsync(int pageSize, string continuationToken);

        public Task UpdateAsync(TEntity entity, string? etag = null);
    }
}
