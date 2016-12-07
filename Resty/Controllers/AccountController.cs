
using Newtonsoft.Json;
using Resty.Authorization;
using Resty.Models;
using System.Web.Http;
namespace Resty.Controllers
{
   
    public class AccountController : ApiController
    {
        [HttpPost]
        public IHttpActionResult LogOn([FromBody]LogOnModel model)
        {
            if(true) //Check valid user
            {
                return Content(System.Net.HttpStatusCode.Forbidden, "Could not authenticate user");
                //return Ok(TokenManager.GenerateToken());
            }
            else
            {
                return Content(System.Net.HttpStatusCode.Forbidden, "Could not authenticate user");
            }
        }

        [AuthenticateTokenAttribute]
        public IHttpActionResult GetYourName()
        {
            string requestingUser = (User as MyPrincipal).UserName;
            return Ok("Your name is: " + requestingUser);
            //return Content(System.Net.HttpStatusCode.Conflict, "Something bad happened");
        }

        [AuthenticateTokenAttribute]
        public IHttpActionResult GetUserInformation()
        {
            return Ok(JsonConvert.SerializeObject(new UserModel() { FirstName = "Ian", LastName = "Weston", Sex = "Male" }));
        }
    }
}
