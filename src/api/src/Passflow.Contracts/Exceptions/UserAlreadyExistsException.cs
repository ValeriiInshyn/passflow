﻿using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passflow.Contracts.Exceptions
{
    public class UserAlreadyExistsException : ApiException
    {
        public UserAlreadyExistsException(string message) : base(message, 400, LogEventLevel.Error)
        {
        }
    }
}
