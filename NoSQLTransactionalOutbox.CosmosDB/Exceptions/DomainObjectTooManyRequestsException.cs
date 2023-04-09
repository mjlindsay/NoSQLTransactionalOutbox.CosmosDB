using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoSQLTransactionalOutbox.CosmosDB.Exceptions
{
    public class DomainObjectTooManyRequestsException : Exception
    {
        public DomainObjectTooManyRequestsException() {
        }

        public DomainObjectTooManyRequestsException(string message) : base(message) {
        }

        public DomainObjectTooManyRequestsException(string message, Exception inner) : base(message, inner) {
        }
    }
}
