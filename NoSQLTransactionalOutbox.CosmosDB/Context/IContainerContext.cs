using Microsoft.Azure.Cosmos;
using NoSQLTransactionalOutbox.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoSQLTransactionalOutbox.CosmosDB.Context
{
    public interface IContainerContext
    {
        public Container Container { get; }

        public void AddOrReplace(IDataPersistenceObject<IEntity> entity);

        public Task<IEnumerable<IDataPersistenceObject<IEntity>>> SaveChangesAsync(CancellationToken cancellationTOken = default);

        public void Reset();
    }
}
