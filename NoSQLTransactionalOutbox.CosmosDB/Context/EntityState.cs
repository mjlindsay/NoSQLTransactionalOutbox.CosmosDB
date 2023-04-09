using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoSQLTransactionalOutbox.CosmosDB.Context
{
    public enum EntityState
    {
        Created,
        Updated,
        Deleted,
        Unmodified
    }
}
