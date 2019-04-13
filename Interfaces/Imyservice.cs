using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebApp_OpenIDConnect_DotNet
{
   public interface Imyservice
    {
        IEnumerable<Claim> GetClaims();
    }
}
