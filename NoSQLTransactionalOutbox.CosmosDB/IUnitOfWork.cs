using NoSQLTransactionalOutbox.Core.Entity;
using NoSQLTransactionalOutbox.CosmosDB.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NoSQLTransactionalOutbox.CosmosDB
{
    public interface IUnitOfWork
    {
        Task<IEnumerable<DataPersistenceObject<IEntity>>> CommitAsync(CancellationToken cancellationToken = default);
    }
}
