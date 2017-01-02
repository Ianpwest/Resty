
using DataManagement.Models;
using Newtonsoft.Json;
using Resty.Authorization;
using Resty.Models;
using Resty.Utilities;
using System.Web.Http;
namespace Resty.Controllers
{
   
    public class AccountController : ApiController
    {
        [HttpPost]
        public IHttpActionResult LogOn([FromBody]LogOnModel model)
        {
            if (model == null)
                return Content(System.Net.HttpStatusCode.BadRequest, new LogOnResultModel() { bSuccessful = false, FailureReason = "Request not formatted correctly." });

            LogOnResultModel logOnResultModel = AccountUtilities.ValidateUserLogOn(model);

            if(logOnResultModel.bSuccessful) //Check valid user
            {
                return Ok(logOnResultModel);
            }
            else
            {
                return Content(System.Net.HttpStatusCode.Forbidden, new LogOnResultModel() { bSuccessful = false, FailureReason = logOnResultModel.FailureReason });
            }
        }

        [HttpPost]
        public IHttpActionResult RegisterAccount([FromBody]AccountModel account)
        {
            if(account == null)
                return Content(System.Net.HttpStatusCode.BadRequest, new ServiceCallResultModel() { bSuccessful = false, FailureReason = "Request not formatted correctly." });

            ServiceCallResultModel serviceCallResultModel = AccountUtilities.RegisterAccount(account);

            if(!serviceCallResultModel.bSuccessful)
            {
                return Content(System.Net.HttpStatusCode.Forbidden, new ServiceCallResultModel() { bSuccessful = false, FailureReason = serviceCallResultModel.FailureReason });
            }

            return Ok(new ServiceCallResultModel() { bSuccessful = true });
        }

        [HttpPost]
        public IHttpActionResult SendPasswordResetEmail([FromBody]ResetPasswordModel resetPasswordModel)
        {
            if(resetPasswordModel == null)
                return Content(System.Net.HttpStatusCode.BadRequest, new ServiceCallResultModel() { bSuccessful = false, FailureReason = "Request not formatted correctly." });

            ServiceCallResultModel serviceCallResultModel = AccountUtilities.GetResetPasswordTokenForUser(resetPasswordModel.Email);

            if (!serviceCallResultModel.bSuccessful)
            {
                return Content(System.Net.HttpStatusCode.Forbidden, new ServiceCallResultModel() { bSuccessful = false, FailureReason = serviceCallResultModel.FailureReason });
            }

            return Ok(new ServiceCallResultModel() { bSuccessful = true });
        }

        [HttpPost]
        public IHttpActionResult ResetPassword([FromBody]ResetPasswordModel resetPasswordModel)
        {
            if (resetPasswordModel == null)
                return Content(System.Net.HttpStatusCode.BadRequest, new ServiceCallResultModel() { bSuccessful = false, FailureReason = "Request not formatted correctly." });

            ServiceCallResultModel serviceCallResultModel = AccountUtilities.ResetPassword(resetPasswordModel);

            if (!serviceCallResultModel.bSuccessful)
            {
                return Content(System.Net.HttpStatusCode.Forbidden, new ServiceCallResultModel() { bSuccessful = false, FailureReason = serviceCallResultModel.FailureReason });
            }

            return Ok(new ServiceCallResultModel() { bSuccessful = true });
        }

        [HttpGet]
        [AuthenticateTokenAttribute]
        public IHttpActionResult GetYourName()
        {
            string requestingUser = (User as MyPrincipal).UserName;

            return Ok(new ServiceCallResultModel() { bSuccessful = true, FailureReason = "Your name is: " + requestingUser });
        }

        
    }
}
