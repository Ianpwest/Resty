using System.Web.Http;
using DataManagement.Models;
using Resty.Models;
using Resty.Utilities;

namespace Resty.Controllers
{
    public class StatusController : ApiController
    {
        public IHttpActionResult GetCurrentServiceStatus()
        {
            //Do some stuff here to check service health

            return Ok(new StatusModel() { Status = "Service running. No issues. Hi!" });

        }

        public IHttpActionResult LogUserActivity([FromBody] UserActivityModel userActivityModel)
        {
            if (userActivityModel == null)
                return Content(System.Net.HttpStatusCode.BadRequest, new ServiceCallResultModel() { bSuccessful = false, FailureReason = "Request not formatted correctly." });

            BeaconUtilities.LogUserActivity(userActivityModel);

            return Ok(new ServiceCallResultModel() { bSuccessful = true });
        }
    }
}
