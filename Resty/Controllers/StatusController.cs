using System.Web.Http;
using Newtonsoft.Json;
using Resty.Models;

namespace Resty.Controllers
{
    public class StatusController : ApiController
    {
        public string GetCurrentServiceStatus()
        {
            //Do some stuff here to check service health

            return JsonConvert.SerializeObject(new StatusModel() { Status = "Service running. No issues" });

        }
    }
}
