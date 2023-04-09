using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoSQLTransactionalOutbox.CosmosDB.Exceptions
{
    public class DomainObjectNotModifiedException : Exception
    {
        public DomainObjectNotModifiedException() {
        }

        public DomainObjectNotModifiedException(string message) : base(message) {
        }

        public DomainObjectNotModifiedException(string message, Exception inner) : base(message, inner) {
        }
    }
}
