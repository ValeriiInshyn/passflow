using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passflow.Contracts.Exceptions
{
    public class TokenNotFoundException : ApiException
    {
        public TokenNotFoundException(string message) : base(message)
        {
        }
    }
}
