using System.Web.Http;
using Newtonsoft.Json;
using Resty.Models;

namespace Resty.Controllers
{
    public class StatusController : ApiController
    {
        public IHttpActionResult GetCurrentServiceStatus()
        {
            //Do some stuff here to check service health

            return Ok(new StatusModel() { Status = "Service running. No issues. Hi Ian!" });

        }
    }
}
