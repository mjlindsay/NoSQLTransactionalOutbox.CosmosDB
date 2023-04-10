using NoSQLTransactionalOutbox.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoSQLTransactionalOutbox.CosmosDB.Tests.Fixtures
{
    public class FakeEntity : IEntity
    {
        public string Id { get; set; } = string.Empty;
    }
}
