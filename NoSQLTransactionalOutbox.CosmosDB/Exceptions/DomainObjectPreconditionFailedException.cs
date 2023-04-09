using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoSQLTransactionalOutbox.CosmosDB.Exceptions
{
    public class DomainObjectPreconditionFailedException : Exception
    {
        public DomainObjectPreconditionFailedException() {
        }

        public DomainObjectPreconditionFailedException(string message) : base(message) {
        }

        public DomainObjectPreconditionFailedException(string message, Exception inner) : base(message, inner) {
        }
    }
}
