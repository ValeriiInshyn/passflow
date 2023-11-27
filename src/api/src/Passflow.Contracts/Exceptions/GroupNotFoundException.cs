using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog.Events;

namespace Passflow.Contracts.Exceptions
{
    public class GroupNotFoundException : ApiException
    {
        public GroupNotFoundException(string message) : base(message, 400, LogEventLevel.Error)
        {
        }
    }
}
