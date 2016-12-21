using Resty.Utilities;
using System.Security.Principal;
using System.Web.Http;

namespace Resty.Authorization
{
    public class AuthenticateTokenAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            string token = actionContext.Request.Headers.Authorization.Parameter.ToString();
            
            if(!AccountUtilities.ValidateTokenExists(token))
            {
                actionContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Forbidden);
            }
            else
            {

                string userName = AccountUtilities.GetUserNameFromToken(token);

                if(userName == null)
                    actionContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Forbidden);

                var identity = new GenericIdentity("UserName", "Forms");
                
                var principal = new MyPrincipal(identity);
                principal.UserName = userName;

                actionContext.RequestContext.Principal = principal;
            }
        }
    }
}