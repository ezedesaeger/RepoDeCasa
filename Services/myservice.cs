using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;

namespace WebApp_OpenIDConnect_DotNet
{
    public class myservice : Imyservice
    {
        public IEnumerable<Claim> GetClaims()
        {
            var eze = new List<Claim>();
            eze.Add(new Claim("myclaim", "eze"));

            return eze;
        }
    }
}
