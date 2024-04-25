using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace TimeTracker.Utilities
{
    public class Authentication
    {
        public static async Task<bool> GetATokenForGraph(string username, string password)
        {
            string clientId = "39205560-1dec-4abb-88ba-cfec3c2d1336";
            string tenantId = "04b31863-f802-42b0-a7a9-be8dde194ef5";
            var authority = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token";
            string[] scopes = new string[] { "https://graph.microsoft.com/.default" };

            IPublicClientApplication app;
            app = PublicClientApplicationBuilder.Create(clientId)
                  .WithAuthority(authority)
                  .Build();

            AuthenticationResult result = null;
            try
            {
                var securePassword = new SecureString();
                foreach (char c in password)
                    securePassword.AppendChar(c);

                result = await app.AcquireTokenByUsernamePassword(scopes, username, securePassword).ExecuteAsync();
                if (result != null)
                {
                    return true;
                }
                return false;
            }
            catch (MsalException)
            {
                return false;
            }
        }
    }
}
