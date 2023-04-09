using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoSQLTransactionalOutbox.CosmosDB.Exceptions
{
    public class DomainObjectConflictException : Exception
    {
        public DomainObjectConflictException() {
        }

        public DomainObjectConflictException(string message) : base(message) {
        }

        public DomainObjectConflictException(string message, Exception inner) : base(message, inner) {
        }
    }
}
