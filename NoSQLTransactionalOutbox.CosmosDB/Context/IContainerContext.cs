using NoSQLTransactionalOutbox.Core.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoSQLTransactionalOutbox.CosmosDB.Context
{
    public interface IContainerContext
    {

        public List<DataPersistenceObject<IEntity>> DataObjects { get; }

        public void Add(DataPersistenceObject<IEntity> entity);

        public Task<List<DataPersistenceObject<IEntity>>> SaveChangesAsync(CancellationToken cancellationTOken = default);

        public void Reset();
    }
}
