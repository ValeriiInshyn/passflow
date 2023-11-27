using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passflow.Contracts.Exceptions
{
    public class UserNotFoundException : ApiException
    {
        public UserNotFoundException(string message) : base(message, 400, LogEventLevel.Error)
        {
        }
    }
}
