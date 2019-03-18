using Microsoft.AspNetCore.Identity;

namespace WebApp_OpenIDConnect_DotNet.Controllers
{
    public class ApplicationUser : IdentityUser
    {
        public string nombre { get; set; }
    }
}