using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoSQLTransactionalOutbox.CosmosDB.Exceptions
{
    public class DomainObjectNotFoundException : Exception
    {
        public DomainObjectNotFoundException() {
        }

        public DomainObjectNotFoundException(string message) : base(message) {
        }

        public DomainObjectNotFoundException(string message, Exception inner) : base(message, inner) {
        }
    }
}
